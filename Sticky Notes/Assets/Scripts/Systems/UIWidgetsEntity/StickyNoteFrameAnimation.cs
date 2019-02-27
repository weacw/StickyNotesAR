
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.painting;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimatfulWidgetion : StatefulWidget
{
    public List<string> _assetList = new List<string>();

    public FrameAnimatfulWidgetion(Key key=null) : base(key)
    {
    }

    public override State createState()
    {

        return new StickyNoteFrameAnimationState();
    }
}


public class StickyNoteFrameAnimationState : SingleTickerProviderStateMixin<FrameAnimatfulWidgetion>
{
     
    Animation<float> _animation;
    AnimationController _controller;


    public override void initState()
    {
        base.initState();

        for (int i = 0; i < 37; i++)
        {
            if (i <= 9)
                widget._assetList.Add("StandardAssets/UIImage/Movedevice/movedevice_0" + i);
            else
                widget._assetList.Add("StandardAssets/UIImage/Movedevice/movedevice_" + i );
        }

        int imageCount = 37;
        int maxTime = 80 * imageCount;
        _controller = new AnimationController(duration: System.TimeSpan.FromMilliseconds(maxTime),vsync:this);
        _controller.addStatusListener((AnimationStatus status) =>
        {
            if (status == AnimationStatus.completed)
            {
                _controller.forward(from: 0f);
            }
        });

        _animation = new FloatTween(begin: 0, end: imageCount).animate(_controller);
        _animation.addListener(() => setState());
        _controller.forward();
    }
    public override void dispose()
    {
        base.dispose();
        _controller.dispose();
    }



    public override Widget build(BuildContext context)
    {
        int idx = (int)Mathf.Floor(_animation.value) % widget._assetList.Count;
        List<Widget> images =new List<Widget>();
        for(var i = 0; i < widget._assetList.Count; i++)
        {
            if (i != idx)
            {
                images.Add(Image.asset(widget._assetList[i], width:0, height:0));
            }
        }

        images.Add(Image.asset(widget._assetList[idx], width: 200, height: 200));

        return new Stack(alignment: Alignment.center, children: images);
    }
}
