[reflection.Assembly]::LoadWithPartialName("system.core")
$pipe = New-Object system.IO.Pipes.NamedPipeClientStream(".","runnerPipe", [System.IO.Pipes.PipeDirection]"InOut") #, PipeDirection.InOut)
$pipe.Connect()
$sw = New-Object System.IO.StreamWriter($pipe)
$sw.AutoFlush = true;
$sw.WriteLine("cmd.exe");
$sw.WriteLine("");
$sw.Flush();
$sr = New-Object System.IO.StreamReader($pipe)
$line = $sr.ReadLine();

$sw.Close();
$sw.Dispose();
$sr.Close();
$sr.Dispose();
$pipe.Close();
$pipe.Dispose();

exit $line
