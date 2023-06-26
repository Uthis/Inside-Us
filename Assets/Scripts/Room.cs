using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    SpriteRenderer bg;
    public Collider2D confiner;
    public List<PointAsset> pointList;
    [SerializeField] List<Sprite> bgList;

    private void Awake()
    {
        bg = GetComponent<SpriteRenderer>();
    }

    public void SetState(int index)
    {
        bg.sprite = bgList[index];
    }

    public void Tele(string name)
    {
        PointAsset point = pointList.First(x => x.nama.ToLower().Equals(name.ToLower()));
        GameManager.instance.playerController.Transport(this, point.point);
    }
    public void Move(string name)
    {
        PointAsset point = pointList.First(x => x.nama.ToLower() == name.ToLower());
        GameManager.instance.playerController.MoveToPoint(point.point);
    }
    public Transform GetPoint(string name)
    {
        PointAsset point = pointList.First(x => x.nama.ToLower() == name.ToLower());
        return point.point;
    }
}

[Serializable]
public class RoomState
{
    public Sprite BG;
    public float LightIntensity;
}
