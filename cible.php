<?php

$answer = $_POST['dep'];  

if (isset($_POST['dep'])) { // Si réponse
  echo nl2br ("Vous avez répondu : " . $answer . " \n ");
  if ($answer == "59") {          
      echo 'Réponse correcte';      
  }
  else {
      echo 'Réponse incorrecte';
  } 
}
else { // Si non réponse
  echo 'Temps écoulé !';
}

function redirect($url, $time) {
    header("Refresh:$time;Url=$url");
}
 
redirect("http://google.fr", 10);

?>