using System;

namespace UnicomTicManagementSystem.Models
{
    public class Room
    {
        private static int _lastReferenceId = 0;

        public Guid Id { get; set; }
        public string RoomName { get; set; }
        public string RoomType { get; set; }
        public int ReferenceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Room()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            ReferenceId = ++_lastReferenceId;
        }

        public static Room CreateRoom(string roomName, string roomType)
        {
            return new Room
            {
                RoomName = roomName,
                RoomType = roomType,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ReferenceId = ++_lastReferenceId
            };
        }

        public static Room CreateRoomWithReferenceId(string roomName, string roomType, int referenceId)
        {
            return new Room
            {
                RoomName = roomName,
                RoomType = roomType,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ReferenceId = referenceId
            };
        }

        public void SetReferenceId(int referenceId)
        {
            ReferenceId = referenceId;
        }
    }
}
