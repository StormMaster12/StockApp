<?php include "../inc/dbinfo.inc"; ?>
<html>
  <body>
    <?php
    
    $connection = mysqli_connect(DB_SERVER, DB_USERNAME, DB_PASSWORD);
    
    if (mysqli_connect_errno()) echo "Failed to Connect to MySql: " . mysql_connect_error();
    
    $database = mysqli_select_db($connection, DB_DATABASE);
    
    if (isset($_POST)) {
		         
    $request_Type = $_POST['requestType'];
    $table = $_POST['tableName'];

    VerifyTable($connection, DB_DATABASE,$table);
        
    if ($request_Type == "getAll") {
          $result = mysqli_query($connection, "SELECT amount, [Runout Date]	, Name FROM '$table' ") or die (mysqli_error($connection));
          sqlToJson($result);                                 
        }
    elseif ($request_Type = "getSpecific") {
          $amount = $_POST['Amount'];
          $expiryDate = $_POST['expiryDate'];
          $Name = $_POST['Name'];
          
          $result = mysqli_query($connection, "SELECT * FROM '$table' WHERE Amount = '$amount' AND ExpiryDate = '$expiryDate'
                                              AND Name = '$Name'") or die (mysqli_error($connection));
          sqlToJson($result);
        }
    elseif ($request_Type = "addProduct") {
          $Name = $_POST['Name'];
          $Pan = $_POST['Pan'];
          $Amount = $_POST['amount'];
          $shortDescription = $_POST['shortDescription'];
          $longDescription = $_POST['longDescription'];
          $currentDate = $_POST['currentDate'];
          $expiryDate = $_POST['expiryDate'];
          
          $query = mysqli_query($connection, "INSERT INTO '$table' (Name, Pan, Amount, shortDescription, longDescription, currentDate, expiryDate)
                                              Values ( '$Name','$Pan','$Amount','$shortDescription','$longDescription','$currentDate'
                                              ,'$expiryDate')") or die (mysqli_error($connection));
        }
    }
	else {
		echo "No Post Data Sent";
	}
	?>
  </body>
</html>

<?php
        
  function sqlToJson($result)
        {
          $sqlRows = array();
          while ($r = mysqli_fetch_assoc($result))
          {
            $rows[] = $r;
          }
          
          echo json_encode($sqlRows);
        }
        
        
  function VerifyTable($connection, $dbName,$table)
    {
      if(!tableExists($table,$dbName,$connection)) {
           echo "Error Table: " . $table . " Does Not Exist.";
         }
    }
        
    function tableExists($table,$dbName,$connection)
         {
           $t = mysqli_real_escape_string($connection, $table);
           $d = mysqli_real_escape_string($connection, $dbName);
           
           $checkTable = mysqli_query($connection,
                                      "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_NAME = '$t'
                                      AND TABLE_SCHEMA = '$d'");
           if (mysqli_num_rows($checkTable) > 0) return true;
           
           return false;
         }
  ?>
