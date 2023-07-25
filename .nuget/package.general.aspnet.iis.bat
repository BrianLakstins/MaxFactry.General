rem Package the library for Nuget

copy ..\MaxFactry.General.AspNet.IIS-NF-2.0\bin\Release\MaxFactry.General.AspNet.IIS*.dll lib\General.AspNet.IIS\net20\
copy ..\MaxFactry.General.AspNet.IIS-NF-4.5.2\bin\Release\MaxFactry.General.AspNet.IIS*.dll lib\General.AspNet.IIS\net452\
copy ..\MaxFactry.General.AspNet.IIS-NF-4.7.2\bin\Release\MaxFactry.General.AspNet.IIS*.dll lib\General.AspNet.IIS\net472\
copy ..\MaxFactry.General.AspNet.IIS-NF-4.8\bin\Release\MaxFactry.General.AspNet.IIS*.dll lib\General.AspNet.IIS\net48\

c:\install\nuget\nuget.exe pack MaxFactry.General.AspNet.IIS.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release  
