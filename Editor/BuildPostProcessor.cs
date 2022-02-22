// Source: https://stackoverflow.com/questions/54370336/from-unity-to-ios-how-to-perfectly-automate-frameworks-settings-and-plist/54370793#54370793

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace GamePack.Editor
{
    public static class BuildPostProcessor
    {

        [PostProcessBuild]
        public static void AddExemptEncryptionKeyToXcodePlist(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS) return;
            
            var plistPath = path + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var rootDict = plist.root;

            Debug.Log("[PostProcessBuild]\tAdding 'ITSAppUsesNonExemptEncryption = false' key to Info.plist.");

            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            File.WriteAllText(plistPath, plist.WriteToString());
        }
        
        /* FOR REFERENCE
        [PostProcessBuild]
        public static void ChangeXcodePlist(BuildTarget buildTarget, string path)
        {

            if (buildTarget == BuildTarget.iOS)
            {

                var plistPath = path + "/Info.plist";
                var plist = new PlistDocument();
                plist.ReadFromFile(plistPath);

                var rootDict = plist.root;

                Debug.Log(">> Automation, plist ... <<");

                // example of changing a value:
                // rootDict.SetString("CFBundleVersion", "6.6.6");

                // example of adding a boolean key...
                // < key > ITSAppUsesNonExemptEncryption </ key > < false />
                rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
        
        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {

            if (target == BuildTarget.iOS)
            {

                var project = new PBXProject();
                var sPath = PBXProject.GetPBXProjectPath(path);
                project.ReadFromFile(sPath);

                var g = project.GetUnityFrameworkTargetGuid();

                ModifyFrameworksSettings(project, g);

                // modify frameworks and settings as desired
                File.WriteAllText(sPath, project.WriteToString());
            }
        }

        static void ModifyFrameworksSettings(PBXProject project, string g)
        {

            // add hella frameworks

            Debug.Log(">> Automation, Frameworks... <<");

            project.AddFrameworkToProject(g, "blah.framework", false);
            project.AddFrameworkToProject(g, "libz.tbd", false);

            // go insane with build settings

            Debug.Log(">> Automation, Settings... <<");

            project.AddBuildProperty(g,
                "LIBRARY_SEARCH_PATHS",
                "../blahblah/lib");

            project.AddBuildProperty(g,
                "OTHER_LDFLAGS",
                "-lsblah -lbz2");

            // note that, due to some Apple shoddyness, you usually need to turn this off
            // to allow the project to ARCHIVE correctly (ie, when sending to testflight):
            project.AddBuildProperty(g,
                "ENABLE_BITCODE",
                "false");
        }*/
    }
}