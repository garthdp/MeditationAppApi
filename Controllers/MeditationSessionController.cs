using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace OPSCApi.Controllers
{
    [Route("api/Meditation")]
    [ApiController]
    public class MeditationSessionController : ControllerBase
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
        public string Post(string email, string date, string description, string title, string meditationTime)
        {
            CollectionReference coll = db.Collection("Users").Document(email).Collection("Sessions");
            DocumentReference docRef = coll.Document();
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"SessionID", docRef.Id},
                {"Date", date},
                {"Title", title},
                {"Description", description },
                {"MeditationTime", meditationTime }
            };
            docRef.SetAsync(data);
            return "Added Journal Entry";
        }
        [HttpGet("GetSessions")]
        public async Task<IActionResult> GetSessions(string email)
        {
            Query qRef = db.Collection("Users").Document(email).Collection("Sessions");
            QuerySnapshot snapshot = await qRef.GetSnapshotAsync();

            if (snapshot == null)
            {
                return NotFound(new { message = "No sessions entries found." });
            }

            List<Dictionary<string, object>> sessions = new List<Dictionary<string, object>>();

            foreach (DocumentSnapshot docsnap in snapshot)
            {
                Dictionary<string, object> entry = docsnap.ConvertTo<Dictionary<string, object>>();

                if (docsnap.Exists)
                {
                    sessions.Add(entry);
                }
            }

            return Ok(sessions);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string email, string sessionId)
        {
            DocumentReference docref = db.Collection("Users").Document(email).Collection("Sessions").Document(sessionId);
            DocumentSnapshot snapshot = await docref.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                await docref.DeleteAsync();
            }
            else
            {
                return NotFound(new { message = "Session not found" });
            }
            return Ok(new { message = "Session deleted" });
        }
    }
}
