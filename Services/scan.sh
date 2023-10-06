#!/bin/bash                                                                                                                                                                                          
   func (){                                                                                          
       local dir="$1"
       for f in `ls $1`                                                                              
       do                                                                                            
           if [ -f "$dir/$f" ] 
           then                                                                                      
              echo "$dir/$f"                                                                 
          elif [ -d "$dir/$f" ]                                                                     
          then                                                                                      
              func "$dir/$f" 
          fi                                                                                                                                                                              
      done                                                                                          
  }                                                                                                 
  func . 
