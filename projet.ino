#include <Constellation.h>
#include <Wire.h>
#include <Adafruit_TSL2561_U.h>
#include <pgmspace.h>
 
Adafruit_TSL2561_Unified tsl(TSL2561_ADDR_FLOAT); 
 
// ESP8266 Wifi
#include <ESP8266WiFi.h>
char ssid[] = "Connectify-AB";
char password[] = "coucoutoi";
 
// Constellation client
Constellation<WiFiClient> constellation("192.168.120.1", 8088, "ESP8266", "DemoISEN", "1234");

void setup() {
  Serial.begin(115200);  delay(10);
 
  pinMode(D2, OUTPUT);
  pinMode(D3, OUTPUT);
  pinMode(D4, OUTPUT);
   
  // Set Wifi mode
  if (WiFi.getMode() != WIFI_STA) {
    WiFi.mode(WIFI_STA);
    delay(10);
  }
 
  // Connecting to Wifi 
  Serial.print("Connecting to ");
  Serial.println(ssid); 
  WiFi.begin(ssid, password); 
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("WiFi connected. IP: ");
  Serial.println(WiFi.localIP());
 
  // Get package settings (see BasicDemo example)
  // Register StateObjectLinks (see StateObjectsDemo example)
  // Register MessageCallbacks (see MessageCallbackDemo example)
  // Describe SO & MC types (see StateObjectsDemo and MessageCallbackDemo example)

  constellation.registerMessageCallback("DriveLED",
    MessageCallbackDescriptor().setDescription("Pilote la led").addParameter<bool>("R","red component")
    .addParameter<bool>("G", "green component"),
    [](JsonObject& json){
      if (json["Data"].is<JsonArray&>()) {
        constellation.writeInfo("Hello %s %s", json["Data"][0].asString(), json["Data"][1].asString());
        digitalWrite(D2, json["Data"][0].as<bool>() ? HIGH : LOW);
        constellation.pushStateObject("RedState", json["Data"][1].as<bool>());
        digitalWrite(D3, json["Data"][1].as<bool>() ? HIGH : LOW);
        constellation.pushStateObject("GreenState", json["Data"][0].as<bool>());
      }
      else {
        constellation.writeInfo("Hello %s", json["Data"].asString()); 
      }
     
    });

  // Declare the package descriptor
  constellation.declarePackageDescriptor();
 
  // WriteLog info
  constellation.writeInfo("Hello!", constellation.getSentinelName()); 

}

void loop() {
   constellation.loop();

}
