$list = kubectl get pods -n spot-services

foreach($line in $list)
{
    #if (!$line.Contains("zipkin") -And !$line.Contains("liquidity-report")) 
    #if ($line.Contains("active-orders"))     
    #{
        $d = $line.Split(" ")[0]; 
        kubectl delete pod $d -n spot-services 
    #}
    
}