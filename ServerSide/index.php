<?php

include_once('php-ga-master/src/autoload.php');


// Ecosystem constants

// this is the current browser version, queries will pass the browser version used.
// and can be compared, as well as flagged,
//each result will feature meta data as well so the client can compare with the latest available and prompt for an update.
define("ECO_BrowserVersion","0.4.5"); 

// globals for stats
$User_EcosystemBrowser_Version = "n/a";
$User_Unity_Version = "n/a";
$User_PlayMaker_Version = "n/a";
$User_OS_Version = "n/a";


$f3 = require('lib/base.php');


$f3->set('DEBUG',1);

$f3->route('GET /wakTest',
    function($f3,$params)
    {   
	    echo "Welcome to PlayMaker Wak Test\n\n";
	    
	    echo "Headers:\n";
	    foreach (getallheaders() as $name => $value) 
	    {
		 	echo "$name: $value\n";
		 }
		 
		 echo "\nGET var dump:\n";
		 var_dump($_REQUEST);
		 
		 echo "\nPOS var dump:\n";
		 var_dump($_REQUEST);
		 
	 }
);



$f3->route('GET /youtube/@type',
    function($f3,$params)
    {   
	    if ($params['type'] == 'intro')
	    {
	    	$f3->reroute('http://youtu.be/hg3SjsNQyUo?t=39s');
	    	die;
	    }
	    
	    $f3->reroute('https://hutonggames.fogbugz.com/default.asp?W1181');
    }
);


$f3->route('GET /link/@type',
    function($f3,$params) 
    {
	    
	   if ($params['type'] == 'wiki')
	   {
	   	$f3->reroute('https://hutonggames.fogbugz.com/default.asp?W1181');
	   	die;
	   }
	   
		if ($params['type'] == 'changelog')
		{
	  		$f3->reroute('https://github.com/jeanfabre/PlayMaker--Ecosystem--Browser/blob/master/Assets/net.fabrejean/Editor/PlayMaker/Ecosystem/Changelog.md');
	  		die;
	  	} 
	    
	    
	   $f3->reroute('https://hutonggames.fogbugz.com/default.asp?W1181');
    }
);



$f3->route('GET /',
   function() 
   {
      echo 'This is PlayMaker Ecosystem REST API. to search for Custom Actions, Templates Samples and Packages, use the <a href="https://hutonggames.fogbugz.com/default.asp?W1181" >Unity Ecosystem browser</a>';
    }
);

//----------------- PROJECT SCANNING REST API

$f3->route('GET /assetsDescription',
   function($f3,$params) 
   {  
	   	$file = 'AssetsDescription.json';
	    
			if (file_exists($file))
			{
				echo file_get_contents($file);
   		}else{
	   		echo 'Error: file not found';
	   	}
	   	
	}
);
$f3->route('POST /assetsDescription',
   function($f3,$params) 
   {  
	   	$file = 'AssetsDescription.json';
	    
			if (file_exists($file))
			{
				echo file_get_contents($file);
   		}else{
	   		echo 'Error: file not found';
	   	}
	   	
	}
);



//----------------- PUBLISHING REST API


$f3->route('POST /publish',
    function($f3,$params) 
    {
	   $action = "n/a";
	   $data_string = "";
	   $url = "https://snipt.net/api/private/snipt/";
	    
	   $ch = GetSniptSearchCurl($url);
	   $header = array
		(  
			'Authorization: ApiKey jeanfabre:9a0958988cdbc6a2551249de023a820c7880c899',                                                                  
			'Content-Type: application/json'                                                                           
		);
		   
		                                 
	   if (isset($_REQUEST["action"]))
		{
			$action = $_REQUEST["action"];
		}
		
		if ("DELETE" == $action)
		{
			$sniptId = $_REQUEST["snipt"];
			$url =  $url . $sniptId . "/";
		}
		
		if ("POST" == $action)
		{
			
			/*
			$data = array(
			"title" => "A Snipt from server",
			 "lexer" => "c#",
			 "tags" => "Ecosystem, PlayMaker, Unity3, ActionCategory.Test",
			 "code" => "this is the code"
			 );    
			 $data_string = json_encode($data);
			 */
			 
			 $data_string = $_REQUEST["payload"];
			                                                                 
			curl_setopt($ch, CURLOPT_POSTFIELDS, $data_string);     
			array_push($header, 'Content-Length: ' . strlen($data_string));                                                                
		}  
	   
	   echo "Perfoming ".$action." on ".$url;
	   
		
		
		//"?format=json&omit_code&omit_stylized";
		
		// {"title": "A snipt", "lexer": "c#","tags":"Ecosystem, PlayMaker, Unity3, ActionCategory.Test", "code":"this is the code"}

		      
		curl_setopt($ch, CURLOPT_URL, $url);                                                     
		curl_setopt($ch, CURLOPT_CUSTOMREQUEST,  $action);                                                                                                                                                                                                          
		curl_setopt($ch, CURLOPT_HTTPHEADER, $header);                                                                                                              
		//curl_setopt($ch, CURLOPT_VERBOSE, true);
		
		$data = curl_exec($ch);
		
		/*
		 $info = curl_getinfo($ch); 
       if ( !empty($info) && is_array($info) )
       { 
          	print_r( $info ); 
          } else { 
                  throw new Exception('Curl Info is empty or not an array');
       }
       */
       
		curl_close($ch);
		print_r( $data );
		
		echo "perfoming ".$action." on " . $url." DONE";
	}
);


	
	


//----------------- SEARCH REST API

$f3->route('POST /search',
    function($f3,$params) 
    {
		print Search($_REQUEST,'');
	}
);

$f3->route('GET /search',
    function($f3,$params) 
    {
		print Search($_REQUEST,'');
	}
);


$f3->route('POST /search/@keywords',
   function($f3,$params) 
   {
		print Search($_REQUEST,$params["keywords"]);
	}
);

$f3->route('GET /search/@keywords',
   function($f3,$params) 
   {
		print Search($_REQUEST,$params["keywords"]);
	}
);

$f3->route('POST /searchAssets/',
   function($f3,$params) 
   {
		print SearchAssets($_REQUEST,"");
	}
);

$f3->route('GET /searchAssets',
   function($f3,$params) 
   {
		print SearchAssets($_REQUEST,"");
	}
);


$f3->route('POST /searchAssets/@keywords',
   function($f3,$params) 
   {
		print SearchAssets($_REQUEST,$params["keywords"]);
	}
);

$f3->route('GET /searchAssets/@keywords',
   function($f3,$params) 
   {
		print SearchAssets($_REQUEST,$params["keywords"]);
	}
);


$f3->route('GET /download',
    function($f3,$params) {

	 $url = "";
	 $type = "Github"; // default to github for legacy browsers.
	 
   if (isset($_GET["type"]))
   {
	   $type = $_GET["type"];
	}
	
	if ($type=="Github")
	{
		$url = "https://raw.github.com/".$_GET['repository']."/master/".$_GET['file'];
		ga_trackDownload($_GET['repository']."/".$_GET['file']);
	}else if ($type == "Snipt")
	{
		$url = $_GET['url'];
		//ga_trackDownload($_GET['repository']."/".$_GET['slug']);
	}
	

	$f3->reroute($url);

	}
);



$f3->route('GET /preview',
    function($f3,$params) {

	//$url = "https://github.com/".$_GET['repository']."/blob/master/".$_GET['file'];
	$url = "https://raw.github.com/".$_GET['repository']."/master/".$_GET['file'];

	ga_trackPreview($_GET['repository']."/".$_GET['file']);
	
	$f3->reroute($url);
	
	
	/*
	$path_parts = pathinfo($_GET['file']);

	
	$ch = curl_init();
	$timeout = 5;
	curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
	curl_setopt($ch, CURLOPT_USERAGENT, "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)");
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
	curl_setopt($ch, CURLOPT_SSL_VERIFYHOST,false);
	curl_setopt($ch, CURLOPT_SSL_VERIFYPEER,false);
	curl_setopt($ch, CURLOPT_MAXREDIRS, 10);
	curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
	curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, $timeout);
	$data = curl_exec($ch);
	curl_close($ch);
  
  //header('Content-Description: File Transfer');
  	header('Content-disposition: attachment; filename="'.$path_parts['basename'].'"');
 	header('Content-Type: application/octet-stream');

	echo $data;
	*/
	}
);




function startsWith($haystack, $needle)
{
    return $needle === "" || strpos($haystack, $needle) === 0;
}

function endsWith($haystack, $needle)
{
    return $needle === "" || substr($haystack, -strlen($needle)) === $needle;
}


function ConcurrentSearch($curls)
{
	$multi = curl_multi_init();
	$results = array();
	 
	// Loop through the curls handle
	// and attach the handles to our multi-request
	foreach ($curls as $ch) {	 
	    curl_multi_add_handle($multi, $ch);
	}
	 
	// While we're still active, execute curl
	$active = null;
	do {
	    $mrc = curl_multi_exec($multi, $active);
	} while ($mrc == CURLM_CALL_MULTI_PERFORM);
	 
	while ($active && $mrc == CURLM_OK) {
	    // Wait for activity on any curl-connection
	    if (curl_multi_select($multi) == -1) {
	      continue;
	    }
	 
	    // Continue to exec until curl is ready to
	    // give us more data
	    do {
	      $mrc = curl_multi_exec($multi, $active);
	    } while ($mrc == CURLM_CALL_MULTI_PERFORM);
	}
	 
	// Loop through the channels and retrieve the received
	// content, then remove the handle from the multi-handle
	foreach ($curls as $curl) {
		$info = curl_getinfo($curl);
	   $results[$info['url']] = curl_multi_getcontent($curl);
	   curl_multi_remove_handle($multi, $curl);
	}
	 
	// Close the multi-handle and return our results
	curl_multi_close($multi);

	return $results;
}



// ------------------------------------- Search Assets -----------------------------------//


function SearchAssets($request,$keywords)
{

	$time_start = microtime(true); 
	
	// load assets definition
	$file = 'AssetsServerDescription.json';
	    
	if (file_exists($file))
	{
		$assetsServerDes = file_get_contents($file);
	}else{
		echo 'Error: Asset Server Description file not found';
		die;
	}
	

	
	$assetsServerDesJson = json_decode($assetsServerDes,true);

	$U3 = true;
	$U4 = true;
	$U5 = true;
	$PB = false;
	
	$searchableAssetsList = array();

	foreach($request as $key=>$val) 
	{ 
		
		if($key == 'repository_mask') 
		{ 
			$U3 = strpos($val, 'U3') !== FALSE;
			$U4 = strpos($val, 'U4') !== FALSE;
			$U5 = strpos($val, 'U5') !== FALSE;
			$PB = strpos($val, 'PB') !== FALSE; 
		}
		if($key == 'searchableAssets' && ! empty($val) ) 
		{ 
			$searchableAssetsList = explode(',',$val);
		}
	} 
	
	
	//$filters = "";
		
	//ga_trackSearch($keywords,$filters);
	
	// the searches results, all concatenated in the following variable
	$items = array();
	
	$QueryTypes = array();
	$QueryAssets = array();
	$QueryCurls = array();
	$QueryResults = array();
	
	if (array_count_values($searchableAssetsList) > 0)
	{
		foreach($searchableAssetsList as $assetName)
		{
			
			if (! array_key_exists ($assetName,$assetsServerDesJson) )
			{
				//echo 'unknown asset <'.$assetName.'>';
				//die;
				continue;
			}
		
			$rawAsset = $assetsServerDesJson[$assetName];
		
			$assetFilter = $rawAsset["Filter"];
		
			// parse github reps
			if (array_key_exists("Github",$rawAsset))
			{
				
				$url_base = "https://api.github.com/search/code?access_token=".getGithubToken()."&q=";
		
				$githubData = (array)$rawAsset['Github'];
			
	
				// get the github reps for the Unity 3 version
				if ($U3 && array_key_exists("Unity 3",$githubData))
				{
					
					$githubReps = (array)$githubData['Unity 3'];
					foreach ($githubReps as $githubRep)
					{
					
						$url = $url_base.urlencode($keywords)."+__ECO__+".$assetFilter."+repo:".$githubRep;
						
						$ch = GetGithubSearchCurl($url);
						$QueryAssets[$url] = $assetName;
						$QueryTypes[$url]  = 'github';
						array_push($QueryCurls,$ch);
					}
				}
				// get the github reps for the Unity 3 version
				if ($U4 && array_key_exists("Unity 4",$githubData))
				{
					$githubReps = (array)$githubData['Unity 4'];
					foreach ($githubReps as $githubRep)
					{
						$url = $url_base.urlencode($keywords)."+__ECO__+".$assetFilter."+repo:".$githubRep;
						
						$ch = GetGithubSearchCurl($url);
						$QueryAssets[$url] = $assetName;
						$QueryTypes[$url] = 'github';
						array_push($QueryCurls,$ch);
					}
				}
				// get the github reps for the Unity 3 version
				if ($U5 && array_key_exists("Unity 5",$githubData))
				{
					$githubReps = (array)$githubData['Unity 5'];
					foreach ($githubReps as $githubRep)
					{
						$url = $url_base.urlencode($keywords)."+__ECO__+".$assetFilter."+repo:".$githubRep;
				
						$ch = GetGithubSearchCurl($url);
						$QueryAssets[$url] = $assetName;
						$QueryTypes[$url] = 'github';
						array_push($QueryCurls,$ch);
					}
				}
			}
			
		}
	}
	
	// multi curl
	
	$QueryResults = ConcurrentSearch($QueryCurls);

	foreach ($QueryResults as $queryUrl=>$queryResult)
	{
		
		// get the type of url
		$queryType = $QueryTypes[$queryUrl];

		$queryAsset = $QueryAssets[$queryUrl];
		
		if ($queryType == 'github')
		{
			$items = array_merge($items,GetGithubSearchResultItems($queryResult,false,$queryAsset));
		}else if ($queryType == 'snipt')
		{
			$items = array_merge($items,GetSniptSearchResultItems($queryResult,false,$queryAsset));
		}
		
	}

	$time_end = microtime(true);
	$execution_time = ($time_end - $time_start)/60;
	
	header('Content-Type: application/json');

	$result = CreateJsonResult($items,$execution_time);
	
	return $result;
}



// ------------------------------------- Search -----------------------------------//




function Search($request,$keywords)
{
	$time_start = microtime(true); 
	
	$_injectMetaData = isset($_GET["injectMetaData"]);
    
	$filters = "";
	
	$contentTypeMask = array();
	array_push($contentTypeMask,"ALL");
	
	$U3 = true;
	$U4 = true;
	$U5 = true;
	$PB = false;
	
	foreach($request as $key=>$val) 
	{ 
		if($key == 'filter') 
		{ 
			$filters =  "+". urlencode($val) ;
		}
		
		if($key == 'repository_mask') 
		{ 
			$U3 = strpos($val, 'U3') !== FALSE;
			$U4 = strpos($val, 'U4') !== FALSE;
			$U5 = strpos($val, 'U5') !== FALSE;
			$PB = strpos($val, 'PB') !== FALSE; 
		}
		
		
		if($key == 'content_type_mask' ) 
		{ 
			$contentTypeMask = array();
			$contentTypeMask = explode('-',$val);
			unset($contentTypeMask[0]);
		}
		
	} 
	
	if (count($contentTypeMask) == 0)
	{
		array_push($contentTypeMask,"X");
	}
		
	ga_trackSearch($keywords,$filters);
	
	$items = array();
	
	foreach($contentTypeMask as $_mask)
	{
		if (strpos($_mask, 'X') !== FALSE)
		{
			//nothing, we search for every type of content
		}
		if (strpos($_mask, 'A') !== FALSE)
		{
			$filters = "+__ACTION__";
		}
		if (strpos($_mask, 'T') !== FALSE)
		{
			$filters = "+__TEMPLATE__";
		}
		if (strpos($_mask, 'S') !== FALSE)
		{
			$filters = "+__SAMPLE__";
		}
		if (strpos($_mask, 'P') !== FALSE)
		{
			$filters = "+__PACKAGE__";
		}
		if (strpos($_mask, 'B') !== FALSE)
		{
			$filters = "+__BETA__";
		}
		
		// will list all the curls and then perform multi curl with all of them.
		$QueryTypes = array();
		$QueryCurls = array();
		$QueryResults = array();
		
		// Snipt Support
		
		 $sniptSearchEnabled = false;
	    if (isset($_GET["EcosystemVersion"]))
	    {
			    $sniptSearchEnabled = version_compare($_GET["EcosystemVersion"], "0.4.1",">"); 
	    }
    
		if ($sniptSearchEnabled)
		{
			// search within public Snipt
			$snipt_url_base =	"https://snipt.net/api/public/snipt/?format=json&omit_code&omit_stylized&q=";
		
			$snip_url = $snipt_url_base.urlencode($keywords)."+__ECO__".$filters;
			
			$ch = GetSniptSearchCurl($snip_url);
			$QueryTypes[$snip_url] = 'snipt';
			array_push($QueryCurls,$ch);
		}
		
		// search within Github

		$url_base = "https://api.github.com/search/code?access_token=".getGithubToken()."&q=";
	
		if ($U3)
		{
			$url = $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMakerCustomActions_U3";
			
			$ch = GetGithubSearchCurl($url);
			$QueryTypes[$url] = 'github';
			array_push($QueryCurls,$ch);
		}
		
		if ($U4)
		{
			$url =  $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMakerCustomActions_U4";
	
			$ch = GetGithubSearchCurl($url);
			$QueryTypes[$url] = 'github';
			array_push($QueryCurls,$ch);
			
			$url =  $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMaker--Unity--UI";
	
			$ch = GetGithubSearchCurl($url);
			$QueryTypes[$url] = 'github';
			array_push($QueryCurls,$ch);
			
			$url =  $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMaker--UnityLearn--2DPack";
	
			$ch = GetGithubSearchCurl($url);
			$QueryTypes[$url] = 'github';
			array_push($QueryCurls,$ch);
		}
		
	  	if ($U5)
		{
			$url = $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:PlayMakerEcosystem/PlayMakerCustomActions_U5";
			
			$ch = GetGithubSearchCurl($url);
			$QueryTypes[$url] = 'github';
			array_push($QueryCurls,$ch);
			
			$url = $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMakerEcosystemPackagesRep_U5";
			
			$ch = GetGithubSearchCurl($url);
			$QueryTypes[$url] = 'github';
			array_push($QueryCurls,$ch);
		}
	  
		if ($PB)
		{
			$url =  $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMakerBetaActions_U4";
	
			$ch = GetGithubSearchCurl($url);
			$QueryTypes[$url] = 'github';
			array_push($QueryCurls,$ch);
		}
	}
	
	// multi curl
	
	$QueryResults = ConcurrentSearch($QueryCurls);

	foreach ($QueryResults as $queryUrl=>$queryResult)
	{
		
		// get the type of url
		$queryType = $QueryTypes[$queryUrl];

		if ($queryType == 'github')
		{
			$items = array_merge($items,GetGithubSearchResultItems($queryResult,$_injectMetaData));
		}else if ($queryType == 'snipt')
		{
			$items = array_merge($items,GetSniptSearchResultItems($queryResult,$_injectMetaData));
		}
		
	}

	$time_end = microtime(true);
	$execution_time = ($time_end - $time_start)/60;
	
	header('Content-Type: application/json');

	$result = CreateJsonResult($items,$execution_time);
	
	return $result;
}


function Search_ORIG($request,$keywords)
{
	$time_start = microtime(true); 
	
//echo "search";
//print_r($request);

	$_injectMetaData = isset($_GET["injectMetaData"]);
    
	$filters = "";
	
	$contentTypeMask = array();
	array_push($contentTypeMask,"ALL");
	
	$U3 = true;
	$U4 = true;
	$U5 = true;
	$PB = false;
	
	foreach($request as $key=>$val) 
	{ 
		if($key == 'filter') 
		{ 
			$filters =  "+". urlencode($val) ;
		}
		
		if($key == 'repository_mask') 
		{ 
			$U3 = strpos($val, 'U3') !== FALSE;
			$U4 = strpos($val, 'U4') !== FALSE;
			$U5 = strpos($val, 'U5') !== FALSE;
			$PB = strpos($val, 'PB') !== FALSE; 
		}
		
		
		if($key == 'content_type_mask' ) 
		{ 
			$contentTypeMask = array();
			$contentTypeMask = explode('-',$val);
			unset($contentTypeMask[0]);
		}
		
	} 
	
	if (count($contentTypeMask) == 0)
	{
		array_push($contentTypeMask,"X");
	}
		
	ga_trackSearch($keywords,$filters);
	
	$items = array();
	
	foreach($contentTypeMask as $_mask)
	{
		if (strpos($_mask, 'X') !== FALSE)
		{
			//nothing, we search for every type of content
		}
		if (strpos($_mask, 'A') !== FALSE)
		{
			$filters = "+__ACTION__";
		}
		if (strpos($_mask, 'T') !== FALSE)
		{
			$filters = "+__TEMPLATE__";
		}
		if (strpos($_mask, 'S') !== FALSE)
		{
			$filters = "+__SAMPLE__";
		}
		if (strpos($_mask, 'P') !== FALSE)
		{
			$filters = "+__PACKAGE__";
		}
		if (strpos($_mask, 'B') !== FALSE)
		{
			$filters = "+__BETA__";
		}
		
		
		// Snipt Support
		
		 $sniptSearchEnabled = false;
	    if (isset($_GET["EcosystemVersion"]))
	    {
			    $sniptSearchEnabled = version_compare($_GET["EcosystemVersion"], "0.4.1",">"); 
	    }
    
		if ($sniptSearchEnabled)
		{
			// search within public Snipt
			$snipt_url_base =	"https://snipt.net/api/public/snipt/?format=json&omit_code&omit_stylized&q=";
		
			$snip_url = $snipt_url_base.urlencode($keywords)."+__ECO__".$filters;
		
			$items = array_merge($items,GetSniptSearchResultItems(PerformSniptSearch($snip_url),$_injectMetaData));	
		}
		
		
		
		// search within Github
		
		$url_base = "https://api.github.com/search/code?access_token=".getGithubToken()."&q=";
	
		if ($U3)
		{
			$url = $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMakerCustomActions_U3";
			$items = array_merge($items,GetGithubSearchResultItems(PerformGithubSearch($url),$_injectMetaData));	
		}
		
		if ($U4)
		{
			$url =  $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMakerCustomActions_U4";
	
			$items = array_merge($items,GetGithubSearchResultItems(PerformGithubSearch($url),$_injectMetaData));
			
			$url =  $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMaker--Unity--UI";
	
			$items = array_merge($items,GetGithubSearchResultItems(PerformGithubSearch($url),$_injectMetaData));
			
			$url =  $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMaker--UnityLearn--2DPack";
	
			$items = array_merge($items,GetGithubSearchResultItems(PerformGithubSearch($url),$_injectMetaData));
		}
		
	  	if ($U5)
		{
			$url = $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:PlayMakerEcosystem/PlayMakerCustomActions_U5";
			$items = array_merge($items,GetGithubSearchResultItems(PerformGithubSearch($url),$_injectMetaData));	
		}
	  
		if ($PB)
		{
			$url =  $url_base.urlencode($keywords)."+__ECO__".$filters."+repo:jeanfabre/PlayMakerBetaActions_U4";
	
			$items = array_merge($items,GetGithubSearchResultItems(PerformGithubSearch($url),$_injectMetaData));
		}
	}
	
	
	
	// process the data to reduce the bandwidth and the github api junk data.
 
	header('Content-Type: application/json');
 
	$time_end = microtime(true);
	$execution_time = ($time_end - $time_start)/60;
	

	$result = CreateJsonResult($items,$execution_time);
	
	return $result;
}


function CreateJsonResult($items,$execution_time)
{
    
    $metaData = array();
    $metaData["ECO_BrowserVersion"] = constant('ECO_BrowserVersion');
    $metaData["ECO_BrowserVersion_package"] = "https://github.com/jeanfabre/PlayMaker--Ecosystem--Browser/blob/master/Packages/Ecosystem.unitypackage?raw=true";

    $json = (object) array('metadata' => $metaData, 'total_count'=> count($items),'ExecutionTime'=>$execution_time,'items' => $items);
    
    return json_encode($json,JSON_PRETTY_PRINT | JSON_UNESCAPED_SLASHES);
}

function GetSniptSearchCurl($url)
{
	
	$ch = curl_init();
	$timeout = 20;
	
	curl_setopt($ch, CURLOPT_URL, $url);
   curl_setopt($ch, CURLOPT_HEADER, false);
   curl_setopt($ch, CURLOPT_HTTPHEADER, array(
    'username: jeanfabre',
    'Authorization: 9a0958988cdbc6a2551249de023a820c7880c899'
    ));
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
	curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
	curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);
	curl_setopt($ch, CURLOPT_MAXREDIRS, 10);
	curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
	curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, $timeout);
	
	return $ch;
}
	
	
function PerformSniptSearch($url)
{
	$ch = GetSniptSearchCurl($url);
	$data = curl_exec($ch);
	curl_close($ch);

	return $data;
}


function GetGithubSearchCurl($url)
{
	$ch = curl_init();
	$timeout = 5;
	curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
	curl_setopt($ch, CURLOPT_USERAGENT, "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)");
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
	curl_setopt($ch, CURLOPT_SSL_VERIFYHOST,false);
	curl_setopt($ch, CURLOPT_SSL_VERIFYPEER,false);
	curl_setopt($ch, CURLOPT_MAXREDIRS, 10);
	curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
	curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, $timeout);
	return $ch;
}


function PerformGithubSearch($url)
{
	$ch = GetGithubSearchCurl($url);
	$data = curl_exec($ch);
	curl_close($ch);

	return $data;	
}



function GetSniptSearchResultItems($rawJson,$asset=false,$injectMetaData=false)
{

	$json = json_decode($rawJson,true);

	$items = array();

	if (!array_key_exists ( 'objects' , $json ) )
	{
		print_r($rawJson);
		die;
		return $items;
	}

	
	$rawItems = (array)$json['objects'];

	foreach ($rawItems as $rawItem)
	{
		$item = array();
		
		$item["name"] = $rawItem["title"].".cs";
		
		$item["type"] = "Action";
		
		$item['slug'] = $rawItem['slug'];
		
		$item["pretty name"] = preg_replace('/(?<=\\w)(?=[A-Z])/'," $1", $rawItem["title"]);
		
		$item["unity_version"] = "3";
		$item["beta"] = "false";
		
		$item["category"] = "";
		
		$tags = $rawItem["tags"];
		
		foreach ($tags as $tag)
		{	
			if (startsWith($tag['name'],"ActionCategory."))
			{		
				$item["category"] = after(".",$tag["name"]);
			}
			
			if (startsWith($tag['name'],"Type."))
			{		
				$item["type"] = after(".",$tag["name"]);
			}
			
			if ($tag["name"] == "Unity4")
			{
				$item["unity_version"]  = "4";
			}
			
			if ($tag["name"] == "Unity5")
			{
				$item["unity_version"]  = "5";
			}
			
			if ($tag["name"] == "Beta")
			{
				$item["beta"]  = "true";
			}
			
		}
		
		$item["path"] = "Assets/PlayMaker Custom Actions/".$item["category"]."/".$item["name"];
		
		
		$rep = array();
		$rep["type"] = "Snipt";
		$rep['id'] = $rawItem['id'];
		
		$rep["raw_url"] = $rawItem["raw_url"];
		$rep["preview_url"] = $rawItem["full_absolute_url"];
		
		$rawUser = $rawItem['user'];
		$owner = array();
		$owner['login'] = $rawUser['username'];
		$owner['id'] = $rawUser['id'];
		
		$rep['owner'] = $owner;
		
		$item["repository"] = $rep;
		
		array_push($items,$item);
    }
	
	return $items;
}



function GetGithubSearchResultItems($rawJson,$injectMetaData=false,$asset=false)
{
	$json = json_decode($rawJson,true);
	

	$items = array();

	if (!array_key_exists ( 'items' , $json ) )
	{
		print_r($rawJson);
		die;
		return $items;
	}

	
	$rawItems = (array)$json['items'];

	foreach ($rawItems as $rawItem) {
		$item = array();
		$item["name"] = $rawItem["name"];
		
		if (is_string($asset))
		{
			$item["asset"] = $asset;
		}
		
		$item["type"] = "Action";

		$filename = strstr($item["name"], '.cs', true);
		
		if ($filename === false)
		{
			$filename = strstr($item["name"], '.template.txt', true);
			$item["type"] = "Template";
		}
		
		if ($filename === false)
		{
			$filename = strstr($item["name"], '.sample.txt', true);
			$item["type"] = "Sample";
		}
		
		if ($filename === false)
		{
			$filename = strstr($item["name"], '.package.txt', true);
			$item["type"] = "Package";
		}
		
		
		
		$item["pretty name"] = preg_replace('/(?<=\\w)(?=[A-Z])/'," $1", $filename);
		
		$item["path"] = $rawItem["path"];
		//$item["sha"] = $rawItem["sha"];
		
		$pathfolders = explode("/", $item["path"]);
		$item["category"] = $pathfolders[count($pathfolders)-2];
		
		
		$rawRepository = $rawItem['repository'];
		$rep = array();
		$rep["type"] = "Github";
		
		$rep['id'] = $rawRepository['id'];
		$rep['name'] = $rawRepository['name'];
		$rep['full_name'] = $rawRepository['full_name'];
		
		if ( endsWith($rep['name'],"3") )
		{
			$item["unity_version"] = "3";
		}elseif ( endsWith($rep['name'],"5") )
		{
			$item["unity_version"] = "5";
		}else
		{
			$item["unity_version"] = "4";
		}
		
		
		if ( strstr($rep['name'],"Beta") )
		{
			$item["beta"] = "true";
		}else{
			$item["beta"] = "false";
		}
		

		
		$rawOwner = $rawRepository['owner'];
		$owner = array();
		$owner['login'] = $rawOwner['login'];
		$owner['id'] = $rawOwner['id'];
		
		$rep['owner'] = $owner;
		
		$item["repository"] = $rep;
		
		
		if ($injectMetaData)
		{
		    $metaData = array();
		    $fileUrl = 'https://raw.github.com/'.str_replace(' ', '%20',$rep['full_name']).'/master/'.str_replace(' ', '%20',$item["path"]);
		    $metaData['filePath'] = $fileUrl;
		   
		    $ch = curl_init();
		    curl_setopt($ch, CURLOPT_URL, $fileUrl);
		    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
		    curl_setopt($ch, CURLOPT_HEADER, 0);
		    curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
		    $metaDataRaw = curl_exec($ch);
		    curl_close($ch);

		   $json = json_decode($metaDataRaw, true);
		    
		    if ($json!=null)
		    {
			$metaData['raw'] =$json;
		    }
		    
		  

		    $item["metaData"] =  $metaData;
		}
		array_push($items,$item);
    }
	
	return $items;
}

// Download the metadata and inject it in the item
function InjectMetaData($item)
{
    
    
}


 function after ($this, $inthat)
 {
     if (!is_bool(strpos($inthat, $this)))
     return substr($inthat, strpos($inthat,$this)+strlen($this));
 };
    

/*
function TrimSearchResult($rawJson)
{


	$json = json_decode($rawJson,true);
	
	$items = array();
	
	$rawItems = $json['items'];
	
	foreach ($rawItems as $rawItem) {
		$item = array();
		$item["name"] = $rawItem["name"];
		$item["path"] = $rawItem["path"];
		//$item["sha"] = $rawItem["sha"];
		
		$pathfolders = explode("/", $item["path"]);
		$item["category"] = $pathfolders[count($pathfolders)-2];
		
		$rawRepository = $rawItem['repository'];
		$rep = array();
		
		
		$rep['id'] = $rawRepository['id'];
		$rep['name'] = $rawRepository['name'];
		$rep['full_name'] = $rawRepository['full_name'];
		
		$rawOwner = $rawRepository['owner'];
		$owner = array();
		$owner['login'] = $rawOwner['login'];
		$owner['id'] = $rawOwner['id'];
		$owner['gravatar_id'] = $rawOwner['gravatar_id'];
		
		$rep['owner'] = $owner;
		
		$item["repository"] = $rep;
		
		array_push($items,$item);
    }
	
	$json['items'] = $items;
	
	return json_encode($json,JSON_PRETTY_PRINT | JSON_UNESCAPED_SLASHES);

}
*/

function ga_trackPreview($file)
{

	if ($_SERVER['HTTP_HOST']=='localhost:8888'){
		return;
	}
	
	
	//Initilize GA Tracker
	$tracker = new UnitedPrototype\GoogleAnalytics\Tracker( 'UA-49111988-1', 'fabrejean.net');
	
	// Assemble Visitor information
	// (could also get unserialized from database)
	$visitor = new UnitedPrototype\GoogleAnalytics\Visitor();
	$visitor->setIpAddress($_SERVER['REMOTE_ADDR']);
	$visitor->setUserAgent("Unity 4.3");
	
	// Assemble Session information
	// (could also get unserialized from PHP session)
	$session = new UnitedPrototype\GoogleAnalytics\Session();
	
	
	$UnityVersionVar = new UnitedPrototype\GoogleAnalytics\CustomVariable(1,"Unity Version","Unity 4.3",1);
	
	
	$tracker->addCustomVariable($UnityVersionVar);
	
	// Assemble Page information
	$page = new UnitedPrototype\GoogleAnalytics\Page('/REST/preview/'.$file);
	$page->setTitle($file);
	$page->setReferrer('/Unity_4_3/PlayMaker_1_7_7/');
	
	// Track page view
	$tracker->trackPageview($page, $session, $visitor);
}


function ga_trackDownload($file)
{

	if ($_SERVER['HTTP_HOST']=='localhost:8888'){
		return;
	}
	
	
	//Initilize GA Tracker
	$tracker = new UnitedPrototype\GoogleAnalytics\Tracker( 'UA-49111988-1', 'fabrejean.net');
	
	// Assemble Visitor information
	// (could also get unserialized from database)
	$visitor = new UnitedPrototype\GoogleAnalytics\Visitor();
	$visitor->setIpAddress($_SERVER['REMOTE_ADDR']);
	$visitor->setUserAgent("Unity 4.3");
	
	// Assemble Session information
	// (could also get unserialized from PHP session)
	$session = new UnitedPrototype\GoogleAnalytics\Session();
	
	
	$UnityVersionVar = new UnitedPrototype\GoogleAnalytics\CustomVariable(1,"Unity Version","Unity 4.3",1);	
	$tracker->addCustomVariable($UnityVersionVar);
	
	if (isset($_GET["UnityVersion"]))
	{

		$UnityVersionVar = new UnitedPrototype\GoogleAnalytics\CustomVariable(1,"UnityVersion",$_GET["UnityVersion"],1);
		$tracker->addCustomVariable($UnityVersionVar);
	}
	
	if (isset($_GET["PlayMakerVersion"]))
	{
		$PlayMakerVersionVar = new UnitedPrototype\GoogleAnalytics\CustomVariable(1,"PlayMakerVersion",$_GET["PlayMakerVersion"],1);
		$tracker->addCustomVariable($PlayMakerVersionVar);
	}
	
	if(isset($_GET["EcosystemVersion"]))
	{
		$EcosystemVersionVar = new UnitedPrototype\GoogleAnalytics\CustomVariable(1,"EcosystemVersion",explode( ' ', $_GET["EcosystemVersion"] )[0],1);
		$tracker->addCustomVariable($EcosystemVersionVar);
	}
	
	// Assemble Page information
	$page = new UnitedPrototype\GoogleAnalytics\Page('/REST/download/'.$file);
	$page->setTitle($file);
	$page->setReferrer('/Unity_4_3/PlayMaker_1_7_7/');
	
	// Track page view
	$tracker->trackPageview($page, $session, $visitor);
}


function ga_trackSearch($search,$filter)
{

	if ($_SERVER['HTTP_HOST']=='localhost:8888'){
		return;
	}
	
	
	//Initilize GA Tracker
	$tracker = new UnitedPrototype\GoogleAnalytics\Tracker( 'UA-49111988-1', 'fabrejean.net');
		// Assemble Session information
	// (could also get unserialized from PHP session)
	$session = new UnitedPrototype\GoogleAnalytics\Session();
	
	
	// Assemble Visitor information
	// (could also get unserialized from database)
	$visitor = new UnitedPrototype\GoogleAnalytics\Visitor();
	$visitor->setIpAddress($_SERVER['REMOTE_ADDR']);
	
	$path = '';
	
	
	if (isset($_GET["UnityVersion"]))
	{

		$UnityVersionVar = new UnitedPrototype\GoogleAnalytics\CustomVariable(1,"UnityVersion",$_GET["UnityVersion"],1);
		$tracker->addCustomVariable($UnityVersionVar);
		
		$path = $path.'Unity '.$_GET["UnityVersion"];
	}
	
	if (isset($_GET["PlayMakerVersion"]))
	{
		$PlayMakerVersionVar = new UnitedPrototype\GoogleAnalytics\CustomVariable(1,"PlayMakerVersion",$_GET["PlayMakerVersion"],1);
		$tracker->addCustomVariable($PlayMakerVersionVar);
		
		$path = $path.'/PlayMaker '.$_GET["PlayMakerVersion"];
	}
	
	if(isset($_GET["EcosystemVersion"]))
	{
		$EcosystemVersionVar = new UnitedPrototype\GoogleAnalytics\CustomVariable(1,"EcosystemVersion",explode( ' ', $_GET["EcosystemVersion"] )[0],1);
		$tracker->addCustomVariable($EcosystemVersionVar);
		
		$visitor->setUserAgent('Ecosystem Browser '+explode( ' ', $_GET["EcosystemVersion"] )[0]);
		$path = $path.'/Eco Browser '.explode( ' ', $_GET["EcosystemVersion"] )[0];
	}
	

	// Assemble Page information
	$page = new UnitedPrototype\GoogleAnalytics\Page('/REST/search/');
	$page->setTitle($search);
	
	// Track page view
	$tracker->trackPageview($page, $session, $visitor);
}


function getGithubToken()
{
	// Ecosystem github account api requests load balancing

	$GithubTokens = array(
		"10787637aa8564aec3aab88e0f0e3609670139bf", // jeanfabre
		"498213cc3dc876345d24a6c3e673b9e020c7e3b2", // jeanfabreLoadBalancing01 - macdev
		"5ff453b50c1f44cac7ea07e0fcd60ea0b21673f3" // jeanfabreLoadBalancing02 - androiddev
	);

	$file = 'LoadBalancingCache.txt';
	    
	$current = 0;
	    
    if (file_exists($file))
    {
	    $current = file_get_contents($file);
    }
    
    $index = $current+1;
    if ($index >= count($GithubTokens))
    {
	    $index = 0;
    }
    
    
	 // Write the contents back to the file
	 file_put_contents($file, $index);
	 
	  return $GithubTokens[$index];
}

$f3->run();
?>