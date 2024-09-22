using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using System.Collections.Generic;

namespace OPSCApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static FirestoreDb db = establishCon();

        public static FirestoreDb establishCon()
        {
            /*
            Code Attribution
            Title: Getting Started with Google Cloud Firestore using C# | English
            Author: The Amazing Codeverse
            Link: https://www.youtube.com/watch?v=ptEtTNQ0dXg&list=PLrb70iTVZjZPEbhCh85VQIpRbQos2Qx3i&index=2
            Usage: Used to see how to connect Firestore database to api
            */

            string path = AppDomain.CurrentDomain.BaseDirectory + @"opscapi.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            FirestoreDb db = FirestoreDb.Create("opscapi");
            return db;
        }
        [HttpGet]
        public async Task<IActionResult> Get(string email)
        {
            /*
            Code Attribution
            Title: C# Firestore Tutorial 3 | How to Retrieve Data | GET Data | English
            Author: The Amazing Codeverse
            Link: https://www.youtube.com/watch?v=SrRrxYBR3s0&list=PLrb70iTVZjZPEbhCh85VQIpRbQos2Qx3i&index=3
            Usage: Used to get data from Firestore database 
            */
            DocumentReference docRef = db.Collection("Users").Document(email);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            Dictionary<string, object> user = new Dictionary<string, object>();

            if (snapshot.Exists)
            {
                user = snapshot.ToDictionary();
            }
            else
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }
        [HttpPost("createUser")]
        public string CreateUser(string email, string name, string surname)
        {
            /*
            Code Attribution
            Title: C# Firestore Tutorial 2 | How to Store Data | SET Data | English
            Author: The Amazing Codeverse
            Link: https://www.youtube.com/watch?v=wUpbpSlYy-Y&list=PLrb70iTVZjZPEbhCh85VQIpRbQos2Qx3i&index=2
            Usage: Used to see how to add data to a Firestore database
            */

            DocumentReference doc = db.Collection("Users").Document(email);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Email", email},
                {"Name", name},
                {"Surname", surname},
                {"Level", 0 },
                {"Experience", 0 }
            };
            doc.SetAsync(data);
            return "added";
        }
        [HttpPatch("updateLevel")]
        public async Task<IActionResult> UpdateLevel(string email, int experience)
        {
            /*
            Code Attribution
            Title: C# Firestore Tutorial 4 | How to Update Data | Override Data | English
            Author: The Amazing Codeverse
            Link: https://www.youtube.com/watch?v=1VJULdXmP6I&list=PLrb70iTVZjZPEbhCh85VQIpRbQos2Qx3i&index=5
            Usage: Used to see how to update document in a Firestore database
            */
            DocumentReference docref = db.Collection("Users").Document(email);
            DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
            Dictionary<string, object> user = new Dictionary<string, object>();
            if (snapshot.Exists)
            {
                user = snapshot.ToDictionary();
            }
            else
            {
                return NotFound(new { message = "User not found" });
            }

            int level = Convert.ToInt32(user["Level"]);
            int userExperience = Convert.ToInt32(user["Experience"]);

            userExperience += experience;

            if (userExperience >= 100)
            {
                level += 1;
                experience = 0;
            }

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Level",  level},
                {"Experience", experience}
            };

            await docref.UpdateAsync(data);
            return Ok(new { message = "User level and experience updated successfully" });
        }
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            /*
            Code Attribution
            Title: C# Firestore Tutorial 5 | How to Delete Data | Remove Data | English
            Author: The Amazing Codeverse
            Link: https://www.youtube.com/watch?v=kTWMffdX24s&list=PLrb70iTVZjZPEbhCh85VQIpRbQos2Qx3i&index=5
            Usage: Used to see how to delete document in a Firestore database
            */
            DocumentReference docref = db.Collection("Users").Document(email);
            DocumentSnapshot snapshot = await docref.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                await docref.DeleteAsync();
            }
            else
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(new { message = "User deleted" });
        }
    }
}
