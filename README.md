# WebsiteStatusCheckerService
Its a windows service which pings the web site periodically and if it is down then it triggers mail to configured recipient list
# WebsiteStatusCheckerService

###############################################################################################################################
Windows Service
Pings the site periodically
If site is down or unreachable, then the service will trigger mail to the recepient list configured in APP.CONFIG
###############################################################################################################################


TO INSTALL THE SERVICE:
--------------------------------------------------
1. Clone the repository
2. Build the project
3. On successfull build, exe file will be generated in Bin -> Debug folder
4. Open visual studio command prompt and
4. Run the below command by replacing the path of the project
5. installutil /i <path to project>\bin\Debug\WebStatusCheckerService.exe

To calidate the insatllation open the service.msc and check the service website status Alert
----------------------------------------------------

TO CHANGE THE SERVICE CONFIGURATION and SITE DETAILS:
------------------------------------------------------------------------------------------------------
1. To change the service name and type
      i. Open ProjectInstaller.Designer.cs file and change the service name object
      ii. StartType object in the same file for the service startup type
      iii. Account type object for the service login account type
      
2. To change the site details
      i. Open the App.Config and find the appsettings with key = "Parameters"
      ii. Its value is in JSON format with escape string formatted. The similar model can be find the project under model folder. 
      iii. It can contain many sites and recepient list seperated by comma. 
