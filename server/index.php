<?php
if($_SERVER['REQUEST_METHOD'] == "POST") {
	/*
	echo "POST: ";
	var_dump($_POST);

	echo "FILES: ";
	var_dump($_FILES);
*/

	error_log(base64_encode(file_get_contents($_FILES['img']['tmp_name'])));

	//shell_exec('eog ' . $_FILES['img']['tmp_name']);
	//echo '<!DOCTYPE html><html><body>';
	//echo '<img src="data:image/jpeg;base64,' . base64_encode(file_get_contents($_FILES['img']['tmp_name'])) . '" />';

	echo "Image gotten!";
/*
	foreach($_FILES['img'] as $v) {
		error_log($v);
	}
	*/
	/*
	$data = $_POST["img"];
	file_put_contents("temp.jpg", $data);
	error_log("Image recieved!");
	echo "Image recieved!";
	*/

} else if ($_SERVER['REQUEST_METHOD'] == "GET") {
	http_response_code(405);
	echo "<!DOCTYPE html><html><body>Error 405: I need a POST request instead of GET</body></html>";
}
?>
