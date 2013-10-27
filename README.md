'PrismRT CodeGen' is a code generation system for setting up Line of Business Windows Store applications that use the 'Prism for Windows Runtime' guidance from the Microsoft Patterns and Practices group. 
(see http://msdn.microsoft.com/en-us/library/windows/apps/xx130643.aspx)
It helps to reduce the repetitious procedures for starting such Windows Store applications. 

Documentation is included in the download at Docs->PrismRT CodeGen.pdf.

Version 1 used only one Visual Studio 2012 Addin to generate a Windows Store app that was based on one table in a SQL Server 2012 Database. This design was very limiting since you could not use the Addin again to change the generated code or add new generated code. 

Version 2 is more flexible because it uses two addins. The PrismStarter addin runs once and creates a basic Windows Store app that does not contain any code based on tables in the database. The second addin (PrismTable) is run to generate code  based on one table in the database. PrismTable can be run multiple times. It can be run to add new generated code based on a different table in the database. It can be run a second time on a table already run to redo the code previously generated. It can be run a second time on a table already run to delete the code previously generated. 

Version 2 is developed to run on Windows 8.1 and Visual Studio 2013.

'PrismRT CodeGen' is a work in progress. The next version will contain a third addin called PrismTree that will generate code based on multiple tables in the database. 
It will target 2 scenarios:
1) Parent-Child or Parent-Children (e.g. a Person table with related Address, Email, Phone tables)
2) Parent-Child-Grandchild (e.g. Customer, Order, LineItem tables)
3) Data will be displayed using Syncfusion Grid and TreeNavigator controls.