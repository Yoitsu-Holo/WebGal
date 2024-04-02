dirictory (){
	local dir="$1"
	for f in `ls $1`
	do
		if [ -f "$dir/$f" ]
		then
			echo "$dir/$f"
		elif [ -d "$dir/$f" ]
		then
			dirictory "$dir/$f"
		fi
	done                                                                                          
}

files (){
	dirictory ./Pages
	dirictory ./Services
	dirictory ./Shared
	dirictory ./Src
	dirictory ./Types
}

files | xargs wc -l