<!DOCTYPE html>
<html>
    <head>
        <link rel="stylesheet" type="text/css" href="style.css" />
    </head>
    <body>
        <div class="box purple">
            <h1>Virtual Augmentation</h1>
            <p>pdemange, colehorner, and NotTheRealJoe<br />
            Professor Ross Sowell</p>
        </div>
        <table class="hfill">
            <tr>
                <td>
                    <div class="box purple vfill" id="people-count-box">
                        <h1>Number of People in View</h1>
                        <span class="bignumber" id="peoplecount">0</span>
                    </div>
                </td>
                <td>
                    <div class="box purple" id="people-list-box">
                        <h1>People in view</h1>
                        <ul id="people-list">
                        </ul>
                    </div>
                </td>
            </tr>
        </table>
        <div class="box purple">
            <div id="images"></div>
        </div>
    </body>
    <script type="text/javascript" src="jquery-3.3.1.js"></script>
    <script type="text/javascript">
        setInterval(function() {
            $.ajax({url: "getInfo.php"}).done(function(data) {
                console.log(data);
                var info = JSON.parse(data);
                $('#peoplecount').html(info.PeopleCount);

                $('#people-list').html("");
                info.PeopleList.forEach(function(person) {
                    var newPerson = $("<li>" + person + "</li>");
                    newPerson.appendTo('#people-list');
                });

                $('#images').html("");
                info.ImageUriList.forEach(function(imageUri){
                    var newImage = $("<img class='face-image' src='" + imageUri + "' />");
                    newImage.appendTo('#images');
                });
            });
        }, 1000);
    </script>
</html>
