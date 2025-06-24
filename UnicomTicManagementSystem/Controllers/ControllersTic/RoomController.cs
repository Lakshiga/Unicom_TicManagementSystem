using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Controllers.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    public class RoomController
    {
        private readonly RoomRepository _roomRepository;

        public RoomController()
        {
            _roomRepository = new RoomRepository();
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            try
            {
                return await Task.Run(() => _roomRepository.GetAll());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving rooms: {ex.Message}", ex);
            }
        }

        public async Task<Room> GetRoomByIdAsync(Guid id)
        {
            try
            {
                return await Task.Run(() => _roomRepository.GetById(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving room: {ex.Message}", ex);
            }
        }

        public async Task AddRoomAsync(Room room)
        {
            try
            {
                if (room == null)
                    throw new ArgumentNullException(nameof(room));

                if (string.IsNullOrWhiteSpace(room.RoomName))
                    throw new ArgumentException("Room name is required.");

                if (string.IsNullOrWhiteSpace(room.RoomType))
                    throw new ArgumentException("Room type is required.");

                // Check if room name already exists
                var existingRoom = await Task.Run(() => _roomRepository.GetByName(room.RoomName));
                if (existingRoom != null)
                    throw new ArgumentException("Room name already exists.");

                await Task.Run(() => _roomRepository.Add(room));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding room: {ex.Message}", ex);
            }
        }

        public async Task UpdateRoomAsync(Room room)
        {
            try
            {
                if (room == null)
                    throw new ArgumentNullException(nameof(room));

                if (room.Id == Guid.Empty)
                    throw new ArgumentException("Room ID is required.");

                if (string.IsNullOrWhiteSpace(room.RoomName))
                    throw new ArgumentException("Room name is required.");

                if (string.IsNullOrWhiteSpace(room.RoomType))
                    throw new ArgumentException("Room type is required.");

                // Check if room name already exists for different room
                var existingRoom = await Task.Run(() => _roomRepository.GetByName(room.RoomName));
                if (existingRoom != null && existingRoom.Id != room.Id)
                    throw new ArgumentException("Room name already exists.");

                await Task.Run(() => _roomRepository.Update(room));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating room: {ex.Message}", ex);
            }
        }

        public async Task DeleteRoomAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("Room ID is required.");

                await Task.Run(() => _roomRepository.Delete(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting room: {ex.Message}", ex);
            }
        }

        public async Task<List<Room>> GetRoomsByTypeAsync(string roomType)
        {
            try
            {
                return await Task.Run(() => _roomRepository.GetRoomsByType(roomType));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving rooms by type: {ex.Message}", ex);
            }
        }

        public async Task<Room> GetRoomByNameAsync(string roomName)
        {
            try
            {
                return await Task.Run(() => _roomRepository.GetByName(roomName));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving room by name: {ex.Message}", ex);
            }
        }
    }
}
