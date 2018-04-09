<?php
foreach($_POST as $key => $value) {
	if(substr($key, 0, 5) == "FILE:") {
		$data = $key;
		break;
	}
}

$data = substr($data, 5);
$data = base64_decode($data);
file_put_contents("temp.jpg", $data);

$result shell_exec("./detect temp.jpg");

echo substr($result, 39);
?>
