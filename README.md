
# This code has been Written by Victor Fedianine in 1 hour

## TextFileGenerator


To run the file generator program, you can execute the following command in the console:

`TextFileSorter.exe input.txt 100000` 

Where `TextFileGenerator.exe` is the name of the compiled executable, `input.txt` is the path to the file with data, and `10000` is number of lines to have in a text.

Note:
We use a buffer size of 81920 when writing to the file, which is larger than the default buffer size. 
We also parallelize the for-loop using the Parallel.For method, dividing the loop into smaller chunks 
and processing each chunk on a separate thread. 
We also use a lock to prevent multiple threads from writing to the file simultaneously, which can cause errors. 
Finally, we use the ASCII encoding for the StreamWriter to improve performance.

## TextFileSorter


To run the sorter program, you can execute the following command in the console:

`TextFileSorter.exe input.txt output.txt` 

Where `TextFileSorter.exe` is the name of the compiled executable, `input.txt` is the path to the input file, and `output.txt` is the path to the output file. Note that for large files, the sorter program may take a significant amount of time and memory to complete.

Please note: A good solution to sort using a low amount of memory is described here:
https://josef.codes/sorting-really-large-files-with-c-sharp/

## Considerations
It is definately possible to write more performant code, using better sorting algorythms and better writing technique, 
but unfortunately due to my time constraints I didn't have time to investigate and write it out.

Also, it would be good to validate input of TextFileGenerator