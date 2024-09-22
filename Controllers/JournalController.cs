using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace OPSCApi.Controllers
{
    [Route("api/Journal")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        public static FirestoreDb db = establishCon();
        public static FirestoreDb establishCon()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"opscapi.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            FirestoreDb db = FirestoreDb.Create("opscapi");
            return db;
        }

        [HttpPost]
        public string Post(string email, string date, string content, string title)
        {
            CollectionReference coll = db.Collection("Users").Document(email).Collection("Journals");
            DocumentReference docRef = coll.Document();
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"EntryId", docRef.Id},
                {"Date", date},
                {"Title", title},
                {"Content", content }
            };
            docRef.SetAsync(data);
            return "Added journal";
        }
        [HttpGet("GetJournalEntries")]
        public async Task<IActionResult> GetJournalEntries(string email)
        {
            /*
            Code Attribution
            Title: C# Firestore Tutorial 3 | How to Retrieve Data | GET Data | English
            Author: The Amazing Codeverse
            Link: https://www.youtube.com/watch?v=SrRrxYBR3s0&list=PLrb70iTVZjZPEbhCh85VQIpRbQos2Qx3i&index=3
            Usage: Used to get collection of data from Firestore database 
            */
            Query qRef = db.Collection("Users").Document(email).Collection("Journals");
            QuerySnapshot snapshot = await qRef.GetSnapshotAsync();

            if (snapshot == null)
            {
                return NotFound(new { message = "No journal entries found." });
            }

            List<Dictionary<string, object>> journalEntries = new List<Dictionary<string, object>>();

            foreach (DocumentSnapshot docsnap in snapshot)
            {
                Dictionary<string, object> entry = docsnap.ConvertTo<Dictionary<string, object>>();

                if (docsnap.Exists)
                {
                    journalEntries.Add(entry);
                }
            }

            return Ok(journalEntries);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string email, string entryId)
        {
            DocumentReference docref = db.Collection("Users").Document(email).Collection("Journals").Document(entryId);
            DocumentSnapshot snapshot = await docref.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                await docref.DeleteAsync();
            }
            else
            {
                return NotFound(new { message = "Journal not found" });
            }
            return Ok(new { message = "Journal deleted" });
        }
    }
}
