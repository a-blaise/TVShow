#include <Constellation.h>

/* Arduino Wifi (ex: Wifi Shield) */
//#include <SPI.h>
//#include <WiFi.h>

/* Arduino Wifi101 (WiFi Shield 101 and MKR1000 board) */
//#include <SPI.h>
//#include <WiFi101.h>

/* Arduino Ethernet */
//#include <SPI.h>
//#include <Ethernet.h>

/* ESP8266 Wifi */
#include <ESP8266WiFi.h>

/* Wifi credentials */
// char ssid[] = "Connectify-AK";
char ssid[] = "Connectify-AB";
// char password[] = "killianESP8266";
char password[] = "coucoutoi";

/* Create the Constellation client */
// Constellation<WiFiClient> constellation("192.168.137.1", 8088, "ESP8266", "QCM", "20a6ce246d11435b05b821a77669eeec31c25bc7");
Constellation<WiFiClient> constellation("192.168.121.1", 8088, "ESP8266", "QCM", "38c2fe74fc46b4a22a9858916e53e6d285608c8f");

void setup(void) {
  Serial.begin(115200);  delay(10);
  
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

  // il reçoit un message (la réponse ici)
  constellation.registerMessageCallback("SendQuestion",
    MessageCallbackDescriptor().setDescription("Envoi de question").addParameter("q", "qr"),
    [](JsonObject& json) { 
      constellation.writeInfo(json["Data"]["Label"]);
   });

  // Declare the package descriptor
  constellation.declarePackageDescriptor();

  // Subscribe to message group
  constellation.subscribeToGroup("ClientQCM"); // clientQCM est le client ESP mais aussi la page web  
}
  
void loop(void) {
  delay(1000); // Best practice : don't wait here, add a counter or ArduinoThreadc
 
  // envoie un message
  constellation.sendMessage(Package, "Quiz", "SendResponse", "2");

  // Process incoming message & StateObject updates
  constellation.loop();
}
