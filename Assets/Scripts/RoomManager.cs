using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public RoomAsset currentRoom;
    public List<RoomAsset> roomList;

    private void Start()
    {
        currentRoom = roomList[0];
    }

    public void SetRoom(string roomName,int state)
    {
        RoomAsset room = roomList.First(x => x.nama.ToLower().Equals(roomName.ToLower()));
        room.room.SetState(state);
    }

    public void Tele(string roomName,string pointName)
    {
        RoomAsset room = roomList.First(x => x.nama.ToLower().Equals(roomName.ToLower()));
        currentRoom = room;
        room.room.Tele(pointName);
    }

    public void Move(string name)
    {
        currentRoom.room.Move(name);
    }

    public Transform GetPoint(string roomName,string pointName)
    {
        RoomAsset room = roomList.First(x => x.nama.ToLower().Equals(roomName.ToLower()));
        return room.room.GetPoint(pointName);
    }

    [Serializable]
    public class RoomAsset
    {
        public string nama;
        public Room room;
    }
}
