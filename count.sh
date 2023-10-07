lines=$(git log --author=YoitsuHolo --pretty=tformat: --numstat | awk '{ printf "%s\n", $1 - $2 }')
for line in $lines
do
	if [ $line -gt 2000 ]
	then
		echo "Ignoring line count for commit with $line lines"
	else
		total_lines=$((total_lines + line))
	fi
done
echo "Total lines: $total_lines"