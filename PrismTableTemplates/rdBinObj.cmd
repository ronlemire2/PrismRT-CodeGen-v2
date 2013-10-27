rem http://superuser.com/questions/201888/how-to-create-batch-file-that-delete-all-the-folders-named-bin-or-obj-recurs/201892#201892
rem for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"
rem OR
rem for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s "%%d"

for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"
pause