rem Package the library for Nuget
copy ..\MaxFactry.General-NF-2.0\bin\Release\MaxFactry.General*.dll lib\General\net20\
copy ..\MaxFactry.General-NF-4.5.2\bin\Release\MaxFactry.General*.dll lib\General\net452\
copy ..\MaxFactry.General-NF-4.7.2\bin\Release\MaxFactry.General*.dll lib\General\net472\
copy ..\MaxFactry.General-NF-4.8\bin\Release\MaxFactry.General*.dll lib\General\net48\
copy ..\MaxFactry.General-NC-2.1\bin\Release\netcoreapp2.1\MaxFactry.General*.dll lib\General\netcoreapp2.1\
copy ..\MaxFactry.General-NC-3.1\bin\Release\netcoreapp3.1\MaxFactry.General*.dll lib\General\netcoreapp3.1\
copy ..\MaxFactry.General-NC-6.0\bin\Release\net6.0\MaxFactry.General*.dll lib\General\net6.0\

c:\install\nuget\nuget.exe pack MaxFactry.General.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 

copy ..\MaxFactry.General.AspNet-NF-2.0\bin\Release\MaxFactry.General.AspNet*.dll lib\General.AspNet\net20\
copy ..\MaxFactry.General.AspNet-NF-4.5.2\bin\Release\MaxFactry.General.AspNet*.dll lib\General.AspNet\net452\
copy ..\MaxFactry.General.AspNet-NF-4.7.2\bin\Release\MaxFactry.General.AspNet*.dll lib\General.AspNet\net472\
copy ..\MaxFactry.General.AspNet-NF-4.8\bin\Release\MaxFactry.General.AspNet*.dll lib\General.AspNet\net48\

c:\install\nuget\nuget.exe pack MaxFactry.General.AspNet.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 

copy ..\MaxFactry.General.AspNet.IIS-NF-2.0\bin\Release\MaxFactry.General.AspNet.IIS*.dll lib\General.AspNet.IIS\net20\
copy ..\MaxFactry.General.AspNet.IIS-NF-4.5.2\bin\Release\MaxFactry.General.AspNet.IIS*.dll lib\General.AspNet.IIS\net452\
copy ..\MaxFactry.General.AspNet.IIS-NF-4.7.2\bin\Release\MaxFactry.General.AspNet.IIS*.dll lib\General.AspNet.IIS\net472\
copy ..\MaxFactry.General.AspNet.IIS-NF-4.8\bin\Release\MaxFactry.General.AspNet.IIS*.dll lib\General.AspNet.IIS\net48\

c:\install\nuget\nuget.exe pack MaxFactry.General.AspNet.IIS.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 

copy ..\MaxFactry.General.AspNet.IIS.Mvc4-NF-4.5.2\bin\MaxFactry.General.AspNet.IIS.Mvc4*.dll lib\General.AspNet.IIS.Mvc4\net452\

c:\install\nuget\nuget.exe pack MaxFactry.General.AspNet.IIS.Mvc4.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 

copy ..\MaxFactry.General.AspNet.IIS.Mvc5-NF-4.5.2\bin\MaxFactry.General.AspNet.IIS.Mvc5*.dll lib\General.AspNet.IIS.Mvc5\net452\
copy ..\MaxFactry.General.AspNet.IIS.Mvc5-NF-4.7.2\bin\MaxFactry.General.AspNet.IIS.Mvc5*.dll lib\General.AspNet.IIS.Mvc5\net472\
copy ..\MaxFactry.General.AspNet.IIS.Mvc5-NF-4.8\bin\MaxFactry.General.AspNet.IIS.Mvc5*.dll lib\General.AspNet.IIS.Mvc5\net48\

c:\install\nuget\nuget.exe pack MaxFactry.General.AspNet.IIS.Mvc5.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 
