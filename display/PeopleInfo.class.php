<?php
class PeopleInfo {
    public $PeopleCount;
    public $PeopleList;
    public $ImageUriList;

    function __construct($directory) {
        $this->PeopleCount = file_get_contents("number.txt");

        //people list names.txt
        $this->PeopleList = explode("\n", file_get_contents("names.txt"));

        //image list folder faces
        $this->ImageUriList = array();
        foreach(scandir("faces") as $faceImage) {
            if($faceImage != '.' && $faceImage != '..') array_push($this->ImageUriList, "faces/" . $faceImage);
        }
    }
}