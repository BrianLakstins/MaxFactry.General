rem Package the library for Nuget
copy ..\MaxFactry.General.AspNet-NF-2.0\bin\Release\MaxFactry.General.AspNet*.dll lib\General.AspNet\net20\
copy ..\MaxFactry.General.AspNet-NF-4.5.2\bin\Release\MaxFactry.General.AspNet*.dll lib\General.AspNet\net452\
copy ..\MaxFactry.General.AspNet-NF-4.7.2\bin\Release\MaxFactry.General.AspNet*.dll lib\General.AspNet\net472\
copy ..\MaxFactry.General.AspNet-NF-4.8\bin\Release\MaxFactry.General.AspNet*.dll lib\General.AspNet\net48\

c:\install\nuget\nuget.exe pack MaxFactry.General.AspNet.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 
