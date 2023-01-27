using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Backup.Data
{
    public class BackupProfile
    {
        /*==================================================*
         *==                   FIELDS                     ==*
         *==================================================*/
        
        private string Name { get; }
        public IList<string> GlobalExcludePaths { get; }
        public IList<BackupLocation> BackupLocations { get; }
        public bool DryRun { get; }

        /*==================================================*
         *==                CONSTRUCTORS                  ==*
         *==================================================*/
        
        public BackupProfile(string name, IList<string> globalExcludePaths, IList<BackupLocation> backupLocations, bool dryRun)
        {
            Name = name;
            GlobalExcludePaths = globalExcludePaths;
            BackupLocations = backupLocations;
            DryRun = dryRun;
        }
        
        /*==================================================*
         *==                 TOSTRING()                   ==*
         *==================================================*/

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BackupProfile '")
                .Append(Name)
                .Append("':")
                .Append(Environment.NewLine)
                .Append(Environment.NewLine);

            sb.Append("Global exclude paths: ")
                .Append(Environment.NewLine);
            foreach (string path in GlobalExcludePaths)
            {
                sb.Append(">>>")
                    .Append(Environment.NewLine)
                    .Append(path);
            }

            sb.Append(Environment.NewLine)
                .Append(Environment.NewLine)
                .Append("Global exclude paths: ")
                .Append(Environment.NewLine);
            foreach (BackupLocation loc in BackupLocations)
            {
                sb.Append(">>>")
                    .Append(Environment.NewLine)
                    .Append(loc);
            }

            return sb.ToString();
        }
    }
}