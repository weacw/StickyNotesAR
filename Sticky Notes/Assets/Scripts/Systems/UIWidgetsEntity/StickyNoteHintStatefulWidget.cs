using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using TextStyle = Unity.UIWidgets.painting.TextStyle;
using Image = Unity.UIWidgets.widgets.Image;
using Color = Unity.UIWidgets.ui.Color;

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
        List<Color> begin = new List<Color>();
        begin.Add(Color.fromARGB(255,79, 172, 254));
        begin.Add(Color.fromARGB(255,0, 242, 254));
        Widget button = new MaterialButton(
        onPressed:()=>Navigator.pop(context),
        child: new Container(
                padding: EdgeInsets.only(left: 10, right: 10, top: 5, bottom: 5),
                decoration: new BoxDecoration(gradient: new LinearGradient(colors: begin), borderRadius: BorderRadius.all(15), shape: BoxShape.rectangle),
                child: new Text("Got It!", style: new TextStyle(color: CLColors.white, fontSize: 18))
            )
        );


        List<Widget> widgets = new List<Widget>();
        widgets.Add(new HintCard("Scan!", "Move your phone to scan the panel", "StandardAssets/UIImage/scan"));
        widgets.Add(new HintCard("Place!", "Place your sticky note", "StandardAssets/UIImage/place",button));

       MediaQueryData mediaQueryData = MediaQuery.of(context);

        return new Stack(
            children:new List<Widget>
            {
                new Container(
                margin:EdgeInsets.only(top:mediaQueryData.size._dy*0.25f,bottom:mediaQueryData.size._dy*0.23f,right:50,left:50),
                decoration: new BoxDecoration(borderRadius:BorderRadius.all(30),color: CLColors.background3, shape: BoxShape.rectangle),
                child:new PageView(
                           onPageChanged: (i) => { Debug.Log(i); },
                           itemCount:2,
                           children: widgets
                       )
                )
            }
        );
    }
}


public class HintCard : StatelessWidget
{
    private string title;
    private string descript;
    private string imagepath;
    private Widget expandWidget;
    public HintCard(string title,string descript,string imagepath,Widget expandWidget=null)
    {
        this.title = title;
        this.descript = descript;
        this.imagepath = imagepath;
        this.expandWidget = expandWidget ?? new Container();
    }



    public override Widget build(BuildContext context)
    {
        return new Container(
                margin: EdgeInsets.all(20),
                child: new Column
                (
                    children: new List<Widget>
                    {
                        new Container(
                                margin:EdgeInsets.all(15),
                                child:Image.asset(imagepath,alignment:Alignment.center,width:200,height:200)
                            ),
                            new Container(
                                child:new Text(this.title,textAlign:TextAlign.left,style:new TextStyle(fontSize:20,fontWeight:FontWeight.w700))
                            ),
                            new Container(
                                child:new Text(this.descript,textAlign:TextAlign.left)
                            ),
                            new Container(
                                margin:EdgeInsets.all(10),
                                child:expandWidget
                            )
                    }
                )
            );
    }
}
