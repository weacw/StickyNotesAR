using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using TextStyle = Unity.UIWidgets.painting.TextStyle;
using Image = Unity.UIWidgets.widgets.Image;

#region StickyNote main view
public class StickyNoteStatefulWidget : StatefulWidget
{
    public StickyNoteStatefulWidget(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new StickyNoteState();
    }
}

public class StickyNoteState : State<StickyNoteStatefulWidget>
{
    public List<Widget> rows = new List<Widget>();

    public override void initState()
    {
        base.initState();
        _getData();
    }
    public override Widget build(BuildContext context)
    {

        var container = new Container(
            color: CLColors.background3,
            child:
                new Column(
                            crossAxisAlignment: Unity.UIWidgets.rendering.CrossAxisAlignment.start,
                            children: new List<Widget>
                            {
                                this._buildHeader(context),
                                this._buildBody(context),
                                this._buildFooter(context)
                            }
                    )
               );
        return container;
    }


    private Widget _buildHeader(BuildContext context)
    {
        return new Container(
                padding: EdgeInsets.only(left: ScreenHelper._getScreenSize(context).x * 0.025f, top: ScreenHelper._getScreenSize(context).y * 0.1f),
                child: new Text("StickyNote Scenes", style: new TextStyle(fontSize: 20.0f, fontWeight: FontWeight.w700))
            );
    }


    private Widget _buildBody(BuildContext context)
    {
        if (rows.Count > 0)
        {
            return new Flexible(
            child: new Container(
                        padding: EdgeInsets.only(top: 30),
                        child: new ListView(
                        physics: new AlwaysScrollableScrollPhysics(),
                        children: rows
                    )
                )
            );
        }
        return new EmptyItem();
    }

    private Widget _buildFooter(BuildContext context)
    {
        return new Container(
            margin: EdgeInsets.only(top: ScreenHelper._getScreenSize(context).y * 0.2f, left: ScreenHelper._getScreenSize(context).x * 0.9f),
            height: 50,
            child: new GestureDetector(
                       onTap: () => 
                       { 
                            Debug.Log("write");
                            Navigator.popAndPushNamed(context, "WriteView");
                       }, 
                       child: new Icon(icon: Icons.write, size: 30)
                    )
            );
    }


    private void _getData()
    {
        rows.Clear();
        List<StickyNotePanelData> tmp_StickyData = StickyNoteManagerSystem.GetStickyNote.m_StickyNoteDatabase.m_StickNoteDatebase;
        for (int i = 0; i < tmp_StickyData.Count; i++)
        {
            rows.Add(new ItemArticle(tmp_StickyData[i]));
        }
    }

}
class EmptyItem : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        MediaQueryData mediaQueryData = MediaQuery.of(context);
        return new Container(
                        margin: EdgeInsets.only(top: mediaQueryData.size._dy / 2 - 180, left: mediaQueryData.size._dx / 2 - 140),
                        child: new Column(
                                children: new List<Widget>
                                {
                                    Image.asset("StandardAssets/UIImage/EmptyPage",width:200,height:200,fit:BoxFit.cover),
                                    new Text("Nothing!!",style:new TextStyle(fontSize:30,fontWeight:FontWeight.w700)),
                                    new Text("Your sticky note panel is empty!",style:new TextStyle(fontSize:20,fontWeight:FontWeight.w400),textAlign:TextAlign.left,textScaleFactor:0.2f),
                                }
                            )
            );
    }
}

class ItemArticle : StatelessWidget
{

    public StickyNotePanelData stickyNotePanelData { get; private set; }
    public ItemArticle(StickyNotePanelData _stickyNotePanelData)
    {
        stickyNotePanelData = _stickyNotePanelData;
    }


    public override Widget build(BuildContext context)
    {
        return new GestureDetector(
            onTap: () =>
            {
                Navigator.popAndPushNamed(context, "ARView");
                NotifCenterSystem.GetNotice.PostDispatchEvent(NotifEventKeyComponent.OnLoadARWorldMap, new AssingObject<StickyNotePanelData>(stickyNotePanelData));
            },
            child: new Container(
            padding: EdgeInsets.only(left: 22),
            height: 80,
            child: new Column(
                    children: new List<Widget>
                    {
                        new Container(
                            width:Screen.width,
                            child:new Text(stickyNotePanelData.m_Title,style:new TextStyle(fontSize:18))),
                        new Container(
                            margin:EdgeInsets.only(top:10),
                            width:Screen.width,
                            child:new Text(stickyNotePanelData.m_Timestamp,style:new TextStyle(fontSize:10))),
                        new Divider(height:16)
                    }
                )
            )
            );
    }

    private void TESTdebug(string str)
    {
        Debug.Log(str);
    }
}

#endregion

