<?php
	header('Content-type: text/html; charset=utf-8');
	require "phpQuery-onefile.php";
	
	$f_json = 'InstUrls.json';
	$json = file_get_contents("$f_json");
	$obj = json_decode($json,true);

	
	foreach ($obj as $value) {

	$ch = curl_init($value);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
	curl_setopt($ch, CURLOPT_HEADER, false); // true - чтобы вывести заголовки 
	$html = curl_exec($ch);
	curl_close($ch); 
	
	echo $html;


	/*Название*/
	$url = $value;
	$file = file_get_contents($url);
	$doc = phpQuery::newDocument($file);
	$name = $doc->find('title')->text();
	$string = serialize($name);
	echo $string;
	file_put_contents('InstData.json', json_encode($string), FILE_APPEND);
	
	/*Количество подписчиков*/
	$metaItems = array();
	foreach(pq('meta[name="description"]') as $meta) {
	$value = pq($meta)->attr('content');
	$metaItems = explode(', ', $value);

}
	unset($metaItems[2]);
	$string = serialize($metaItems);
	echo $string;
	file_put_contents('InstData.json', json_encode($string), FILE_APPEND);

	/*Ссылки и лайки*/
	$data = array();
	 
	preg_match_all('/<script type="text\/javascript">window\._sharedData = \{(.*)\};<\/script>/ism', $html, $matches);
	if (!empty($matches[1][0])) {
		$res = json_decode('{' . $matches[1][0] . '}', true);
	 
		$media = $res['entry_data']['ProfilePage'][0]['graphql']['user']['edge_owner_to_timeline_media']['edges'];
		if (!empty($media)) {
			foreach ($media as $row) {
				//print_r($row['node']);
				$data[] = array(
					'link'       => $row['node']['shortcode'],
					'likes'    => $row['node']['edge_liked_by']['count'],
				);
				unset($data[1], $data[2], $data[3], $data[4], $data[5], $data[6], $data[7], $data[8], $data[9], $data[10], $data[11]);
			}
		}
	}
	$string = serialize($data);
	echo $string;
	file_put_contents('InstData.json', json_encode($string), FILE_APPEND);
}
?>
