@ECHO OFF

rem 1. File name( supports pdf/jpg) OR specify the folder name to process all(pdf/jpg) files, in that case the file name will print in prefix for each result.
rem 2. output type , bydefault JSON, or specify CSV, anything else it will be default

set arg1=%1
set arg2=%2
set arg=BFE
set CLASSPATH=%CLASSPATH%.;lib/*;OCRExt.jar
java  -Xms512m -Xmx1024m -Xnoclassgc com.iqss.ocr.OCRExtraction  %arg1% %arg2% %arg%