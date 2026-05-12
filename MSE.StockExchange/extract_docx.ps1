Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::OpenRead('D:\Projects\My Project\DotnetCore\MSE.StockExchange\wwwroot\images\BRS-PREF ISSUE_Revised_07APR26 FINAL_ISHA.docx')
$entry = $zip.GetEntry('word/document.xml')
$stream = $entry.Open()
$reader = New-Object System.IO.StreamReader($stream)
$xml = $reader.ReadToEnd()
$reader.Close()
$stream.Close()
$zip.Dispose()
$text = $xml -replace '<w:p[ >]', "
"
$text = $text -replace '<[^>]+>', ''
Write-Output $text
