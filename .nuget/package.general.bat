rem Package the library for Nuget
copy ..\MaxFactry.General-NF-2.0\bin\Release\MaxFactry.General*.dll lib\General\net20\
copy ..\MaxFactry.General-NF-4.5.2\bin\Release\MaxFactry.General*.dll lib\General\net452\
copy ..\MaxFactry.General-NF-4.7.2\bin\Release\MaxFactry.General*.dll lib\General\net472\
copy ..\MaxFactry.General-NF-4.8\bin\Release\MaxFactry.General*.dll lib\General\net48\
copy ..\MaxFactry.General-NC-2.1\bin\Release\netcoreapp2.1\MaxFactry.General*.dll lib\General\netcoreapp2.1\
copy ..\MaxFactry.General-NC-3.1\bin\Release\netcoreapp3.1\MaxFactry.General*.dll lib\General\netcoreapp3.1\
copy ..\MaxFactry.General-NC-6.0\bin\Release\net6.0\MaxFactry.General*.dll lib\General\net6.0\

c:\install\nuget\nuget.exe pack MaxFactry.General.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 