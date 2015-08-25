using System;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace UpdateWorkSpaceMappings
{
  internal class Program
  {
    private static void Main(string[] args)
    {

      try
      {
        Console.WriteLine("\r\nEnter the URL for your TFS Server: (ie: http://tfs:8080/tfs)");
        var server = Console.ReadLine();

        Console.WriteLine("\r\nEnter the Name of your old computer and hit enter \r\n (leave blank to skip migrating tfs mappings and exit):");
        var oldComputerName = Console.ReadLine();
        if (!string.IsNullOrEmpty(oldComputerName) && !string.IsNullOrEmpty(server))
        {
          var computername = Environment.MachineName;
          Console.WriteLine("Old ComputerName: {0}", oldComputerName);
          Console.WriteLine("New ComputerName: {0}", computername);

          Console.WriteLine("Connecting to server...");
          TfsTeamProjectCollection tfsCollection = new TfsTeamProjectCollection(new Uri(server));
          VersionControlServer versionControlServer = tfsCollection.GetService<VersionControlServer>();
          Console.WriteLine("Getting list of workspaces...");
          Workspace[] workspaces = versionControlServer.QueryWorkspaces(null, versionControlServer.AuthorizedUser,
                                                                        oldComputerName);
          if (workspaces.Any())
          {

            foreach (var workspace in workspaces)
            {
              Console.WriteLine("Updating workspace: {0}", workspace.Name);
              workspace.Update(
                workspace.Name, //Keep the name
                workspace.OwnerName, //Keep the owner
                workspace.Comment, //Keep the comment
                computername, //New Computer name
                workspace.Folders, //Keep the mappings
                workspace.PermissionsProfile, //Keep the permissions
                false); //May not need to fix mappings ... if not, this will be false. 
            }
          }
          else
          {
            Console.WriteLine("No workspaces Found for that computer name.");
          }
        }

        Console.WriteLine("All Finished! Press any key to continue.");
        Console.ReadKey();

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

    }
  }
}
