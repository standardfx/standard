<#
	Usage:
		1. Copy build output to clipboard
		2. Run this script in PowerShell
		3. Paste clipboard data to notepad and save

	Next Step:
		Analyze the data saved using the benchmark visualization script.
#>

$data = Get-Clipboard

$out = '===[ NET462 ]======================================================='

$out += for ($i = 0; $i -lt $data.Count; $i++)
{
	if ($data[$i] -eq 'Test Run Successful')
	{
		'===[ NETSTANDARD ]======================================================='
		continue
	}

	if (-not $data[$i].Contains('[TOUT/PERF]'))
	{
		continue
	}

	$lines = @()
	for ($j = $i; $j -lt $data.Count; $j++)
	{
		if ([string]::IsNullOrEmpty($data[$j]))
		{
			$i = $j
			break
		}

		$lines += $data[$j]
	}

	$lines
	'----------------------'
}

$out | Set-Clipboard
