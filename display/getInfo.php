<?php
require("PeopleInfo.class.php");

$peopleInfo = new PeopleInfo(".");
echo json_encode($peopleInfo);