using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


#region StickyNote AR view
public class StickyNoteARViewStatefulWidget : StatefulWidget
{
    public StickyNoteARViewStatefulWidget(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new StickyNoteARViewState();
    }
}

public class StickyNoteARViewState : State<StickyNoteARViewStatefulWidget>
{
    bool _foundPanel = false;
    public override void initState()
    {
        base.initState();
        StickyNoteManagerSystem.GetStickyNote.OnFoundPanelCallBack += onFoundPanelCallBack;
        Navigator.pushName(context, "HintView");
    }

    public override void dispose()
    {
        base.dispose();
        StickyNoteManagerSystem.GetStickyNote.OnFoundPanelCallBack -= onFoundPanelCallBack;
    }



    private void onFoundPanelCallBack(bool _found)
    {
        this.setState();
        this._foundPanel = _found;
    }



    public override Widget build(BuildContext context)
    {
        return new Container(
            child: new Column(
                children: new List<Widget>
                    {
                      this._buidHeader(context),
                      this._buildFooter(context),
                    }
                )
            );
    }



    private Widget _buidHeader(BuildContext context)
    {
        return new Container(
                width: Screen.width,
                padding: EdgeInsets.only(left: -260, top: 70),
                child: new GestureDetector(
                    onTap: () =>
                    {
                        Navigator.popAndPushNamed(context, "StickyNoteView");
                        NotifCenterSystem.GetNotice.PostDispatchEvent(NotifEventKeyComponent.OnSaveARWorldMap, new AssingObject<string>("test"));
                    },
                    child: new Icon(icon: Icons.back, size: 30)
                )
            );
    }

    private Widget _buildFooter(BuildContext context)
    {
        if(_foundPanel)
        {
            return new Container(
                    margin: EdgeInsets.only(top: 450),
                    child: new GestureDetector(
                            onTap: () => {
                                NotifCenterSystem.GetNotice.PostDispatchEvent(NotifEventKeyComponent.OnCreateStickNote, new AssingObject<string, ARRaycastHit>("", FocusSquareSystem.GetRayHit));
                           },
                            child: new Icon(icon: Icons.place, size: 50)
                        )
                   );
        }
        else
        {
            return new Container();
        }
    }




}
#endregion