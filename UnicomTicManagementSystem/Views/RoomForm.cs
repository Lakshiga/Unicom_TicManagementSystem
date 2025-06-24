using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Views
{
    public partial class RoomForm : Form
    {
        private RoomController _roomController;
        private Guid selectedRoomId = Guid.Empty;

        public RoomForm()
        {
            InitializeComponent();
            _roomController = new RoomController();
            LoadRoomsAsync();
        }

        private async Task LoadRoomsAsync()
        {
            dgvRooms.DataSource = await _roomController.GetAllRoomsAsync();
            dgvRooms.ClearSelection();
        }     

        private void dgvRooms_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count > 0)
            {
                var room = dgvRooms.SelectedRows[0].DataBoundItem as Room;
                if (room != null)
                {
                    selectedRoomId = room.Id;
                    txtRoomName.Text = room.RoomName; 
                    txtRoomType.Text = room.RoomType; 
                }
            }
        }

        private void ClearFields()
        {
            selectedRoomId = Guid.Empty;
            txtRoomName.Clear();
            txtRoomType.Clear();
        }

        private async void btnAdd_Click_1(object sender, EventArgs e)
        {
            var room = new Room
            {
                RoomName = txtRoomName.Text.Trim(),
                RoomType = txtRoomType.Text.Trim()
            };

            await _roomController.AddRoomAsync(room);
            await LoadRoomsAsync();
            ClearFields();
        }

        private async void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (selectedRoomId == Guid.Empty)
            {
                MessageBox.Show("Please select a room to update.");
                return;
            }

            var room = new Room
            {
                Id = selectedRoomId,
                RoomName = txtRoomName.Text.Trim(),
                RoomType = txtRoomType.Text.Trim()
            };

            await _roomController.UpdateRoomAsync(room);
            await LoadRoomsAsync();
            ClearFields();
        }

        private async void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (selectedRoomId == Guid.Empty)
            {
                MessageBox.Show("Please select a room to delete.");
                return;
            }

            await _roomController.DeleteRoomAsync(selectedRoomId);
            await LoadRoomsAsync();
            ClearFields();
        }
    }
}
