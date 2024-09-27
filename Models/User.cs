using Google.Cloud.Firestore;

namespace OPSCApi.Models
{
    [FirestoreData]
    public class User
    {
        public User() { }
        [FirestoreProperty]
        public string Email { get; set; }
        [FirestoreProperty]
        public string Username { get; set; }
    }
}
