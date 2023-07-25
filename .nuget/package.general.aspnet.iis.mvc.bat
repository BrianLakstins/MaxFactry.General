rem Package the library for Nuget
copy ..\MaxFactry.General.AspNet.IIS.Mvc4-NF-4.5.2\bin\MaxFactry.General.AspNet.IIS.Mvc4*.dll lib\General.AspNet.IIS.Mvc4\net452\

c:\install\nuget\nuget.exe pack MaxFactry.General.AspNet.IIS.Mvc4.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 

copy ..\MaxFactry.General.AspNet.IIS.Mvc5-NF-4.5.2\bin\MaxFactry.General.AspNet.IIS.Mvc5*.dll lib\General.AspNet.IIS.Mvc5\net452\
copy ..\MaxFactry.General.AspNet.IIS.Mvc5-NF-4.7.2\bin\MaxFactry.General.AspNet.IIS.Mvc5*.dll lib\General.AspNet.IIS.Mvc5\net472\
copy ..\MaxFactry.General.AspNet.IIS.Mvc5-NF-4.8\bin\MaxFactry.General.AspNet.IIS.Mvc5*.dll lib\General.AspNet.IIS.Mvc5\net48\

c:\install\nuget\nuget.exe pack MaxFactry.General.AspNet.IIS.Mvc5.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 
