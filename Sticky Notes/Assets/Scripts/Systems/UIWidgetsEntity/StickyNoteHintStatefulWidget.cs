using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using TextStyle = Unity.UIWidgets.painting.TextStyle;
using Image = Unity.UIWidgets.widgets.Image;

public class StickyNoteHintStatefulWidget : StatefulWidget
{
    public StickyNoteHintStatefulWidget(Key key=null) : base(key)
    {
    }

    public override State createState()
    {
        return new StickyNoteHintState();
    }
}



public class StickyNoteHintState : State<StickyNoteHintStatefulWidget>
{

    public override Widget build(BuildContext context)
    {
        return _buildWindow();
    }


    Widget _buildWindow() 
    {
        return new Container(
                margin:EdgeInsets.only(top:120,bottom:120,right:50,left:50),
                decoration: new BoxDecoration(borderRadius:BorderRadius.all(30),color: CLColors.background3, shape: BoxShape.rectangle),
                child:new PageView(
                       children:new List<Widget>
                                {
                                  new HintCard("First","First descript","StandardAssets/UIImage/movedevice"),
                                  new HintCard("Second","Second descript","StandardAssets/UIImage/movedevice"),
                                  new HintCard("Third","Third descript","StandardAssets/UIImage/movedevice"),
                                }
                       )
                );
    }
}


public class HintCard : StatelessWidget
{
    private string title;
    private string descript;
    private string imagepath;

    public HintCard(string title,string descript,string imagepath)
    {
        this.title = title;
        this.descript = descript;
        this.imagepath = imagepath;
    }



    public override Widget build(BuildContext context)
    {
        return new Container(
                margin: EdgeInsets.all(20),
                child: new Column
                (
                    children: new List<Widget>
                    {
                        Image.asset(imagepath),
                        new Text(this.title),
                        new Text(this.descript),
                    }
                )
            );
    }
}