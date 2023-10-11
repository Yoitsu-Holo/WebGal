#!/bin/bash                                                                                                                                                                                          
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
	dirictory ./Src
	dirictory ./Shared
}

files | xargs wc -l