<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
    <title>Questionnaire</title>
  </head>
  <body>
    <p>
      <button onclick="startTimeOut(), showQuestion()">Montrer la question</button>
    </p>
    <form method="post" action="cible.php">
      <fieldset>
        <legend>Question 1</legend> 
        <div id="questionToHide" style="display:none;" >
          <p>
             Quel est le département du nord ?<br />
             <input type="radio" name="dep" value="49" id="49" /> <label for="49">Le 49</label><br />
             <input type="radio" name="dep" value="59" id="59" /> <label for="59">Le 59</label><br />
             <input type="radio" name="dep" value="62" id="62" /> <label for="62">Le 62</label><br />
             <input type="radio" name="dep" value="69" id="69" /> <label for="69">Le 69</label><br />
          </p>
        </div>
      </fieldset>  
      <p>
        <input id="button" type="submit" value="Envoyer" />
      </p>
    </form>
    <div id="bip" class="display"></div>

    <script>
      var counter = 10;
      var intervalId = null;
      document.getElementById("button").disabled = false;

      function action() {
        clearInterval(intervalId);
        document.getElementById("bip").innerHTML = "TERMINE!"; 
        document.getElementById("button").disabled = true;
      }
      function bip() {
        document.getElementById("bip").innerHTML = counter + " secondes restantes";
        counter--;
      }
      function startTimeOut() {
        intervalId = setInterval(bip, 1000);
        setTimeout(action, counter * 1000);
      } 
    </script>

    <script>
      function showQuestion() {
        document.getElementById('questionToHide').style.display = "block";
      }
    </script>
  </body>
</html>