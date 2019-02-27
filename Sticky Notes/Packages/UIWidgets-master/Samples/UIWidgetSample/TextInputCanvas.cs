﻿using System.Collections.Generic;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.service;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;
using TextStyle = Unity.UIWidgets.painting.TextStyle;

namespace UIWidgetsSample {
    public class TextInputCanvas : WidgetCanvas {
        public class TextInputSample : StatefulWidget {
            public readonly string title;

            public TextInputSample(Key key = null, string title = null) : base(key) {
                this.title = title;
            }

            public override State createState() {
                return new _TextInputSampleState();
            }
        }

        protected override Widget getWidget() {
            return new EditableInputTypeWidget();
            // return new TextInputSample(key: null, title: this.gameObject.name);
        }

        class _TextInputSampleState : State<TextInputSample> {
            TextEditingController titleController = new TextEditingController("");
            TextEditingController descController = new TextEditingController("");
            FocusNode _titleFocusNode;
            FocusNode _descFocusNode;

            public override void initState() {
                base.initState();
                this._titleFocusNode = new FocusNode();
                this._descFocusNode = new FocusNode();
            }

            public override void dispose() {
                this._titleFocusNode.dispose();
                this._descFocusNode.dispose();
                base.dispose();
            }

            Widget title() {
                return new Container(child: new Text(this.widget.title ?? "", textAlign: TextAlign.center,
                        style: new TextStyle(fontSize: 24, fontWeight: FontWeight.w700)),
                    margin: EdgeInsets.only(bottom: 20));
            }

            Widget titleInput() {
                return new Row(
                    children: new List<Widget>(
                    ) {
                        new SizedBox(width: 100, child: new Text("Title")),
                        new Flexible(child: new Container(
                            decoration: new BoxDecoration(border: Border.all(new Color(0xFF000000), 1)),
                            padding: EdgeInsets.fromLTRB(8, 0, 8, 0),
                            child: new EditableText(maxLines: 1,
                                controller: this.titleController,
                                selectionControls: MaterialUtils.materialTextSelectionControls,
                                autofocus: true,
                                focusNode: new FocusNode(),
                                style: new TextStyle(
                                    fontSize: 18,
                                    height: 1.5f,
                                    color: new Color(0xFF1389FD)
                                ),
                                selectionColor: Color.fromARGB(255, 255, 0, 0),
                                cursorColor: Color.fromARGB(255, 0, 0, 0))
                        )),
                    }
                );
            }

            Widget descInput() {
                return new Container(
                    margin: EdgeInsets.fromLTRB(0, 10, 0, 10),
                    child: new Row(
                        children: new List<Widget>(
                        ) {
                            new SizedBox(width: 100, child: new Text("Description")),
                            new Flexible(child: new Container(
                                height: 200,
                                decoration: new BoxDecoration(border: Border.all(new Color(0xFF000000), 1)),
                                padding: EdgeInsets.fromLTRB(8, 0, 8, 0),
                                child: new EditableText(maxLines: 200,
                                    controller: this.descController,
                                    selectionControls: MaterialUtils.materialTextSelectionControls,
                                    focusNode: new FocusNode(),
                                    style: new TextStyle(
                                        fontSize: 18,
                                        height: 1.5f,
                                        color: new Color(0xFF1389FD)
                                    ),
                                    selectionColor: Color.fromARGB(255, 255, 0, 0),
                                    cursorColor: Color.fromARGB(255, 0, 0, 0))
                            )),
                        }
                    ));
            }

            public override Widget build(BuildContext context) {
                var container = new Container(
                    padding: EdgeInsets.all(10),
                    decoration: new BoxDecoration(color: new Color(0x7F000000),
                        border: Border.all(color: Color.fromARGB(255, 255, 0, 0), width: 5),
                        borderRadius: BorderRadius.all(2)),
                    child: new Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: new List<Widget> {
                            this.title(),
                            this.titleInput(),
                            this.descInput(),
                        }
                    )
                );
                return container;
            }
        }
    }

    public class EditableInputTypeWidget : StatelessWidget {
        Widget rowWidgets(string title, Widget widget) {
            return new Container(
                height: 80,
                //width:300,
                child: new Row(
                    children: new List<Widget> {
                        new Container(width: 100, child: new Text(title)),
                        new Flexible(child: new Container(child: widget, padding: EdgeInsets.all(4), decoration:
                            new BoxDecoration(border: Border.all(color: Color.black))))
                        // widget,
                    }
                ));
        }

        void textSubmitted(string text) {
            Debug.Log($"text submitted {text}");
        }

        public override Widget build(BuildContext context) {
            List<Widget> widgets = new List<Widget>();
            var style = new TextStyle();
            var cursorColor = new Color(0xFF000000);
            widgets.Add(this.rowWidgets("Default", new EditStateProvider(builder: ((buildContext, controller, node) =>
                new EditableText(controller, node, style, cursorColor, onSubmitted: this.textSubmitted)))));

            widgets.Add(this.rowWidgets("Multiple Line", new EditStateProvider(
                builder: ((buildContext, controller, node) =>
                    new EditableText(controller, node, style, cursorColor, maxLines: 4,
                        onSubmitted: this.textSubmitted)))));

            widgets.Add(this.rowWidgets("ObscureText", new EditStateProvider(
                builder: ((buildContext, controller, node) =>
                    new EditableText(controller, node, style, cursorColor, obscureText: true,
                        onSubmitted: this.textSubmitted)))));

            widgets.Add(this.rowWidgets("Number", new EditStateProvider(builder: ((buildContext, controller, node) =>
                new EditableText(controller, node, style, cursorColor, keyboardType: TextInputType.number,
                    onSubmitted: this.textSubmitted)))));

            widgets.Add(this.rowWidgets("Phone", new EditStateProvider(builder: ((buildContext, controller, node) =>
                new EditableText(controller, node, style, cursorColor, keyboardType: TextInputType.phone,
                    onSubmitted: this.textSubmitted)))));

            widgets.Add(this.rowWidgets("Email", new EditStateProvider(builder: ((buildContext, controller, node) =>
                new EditableText(controller, node, style, cursorColor, keyboardType: TextInputType.emailAddress,
                    onSubmitted: this.textSubmitted)))));

            widgets.Add(this.rowWidgets("Url", new EditStateProvider(builder: ((buildContext, controller, node) =>
                new EditableText(controller, node, style, cursorColor, keyboardType: TextInputType.url,
                    onSubmitted: this.textSubmitted)))));

            return new Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: widgets);
        }
    }

    public class EditStateProvider : StatefulWidget {
        public delegate EditableText EditableBuilder(BuildContext context,
            TextEditingController controller, FocusNode focusNode);

        public readonly EditableBuilder builder;

        public EditStateProvider(Key key = null, EditableBuilder builder = null) : base(key) {
            D.assert(builder != null);
            this.builder = builder;
        }

        public override State createState() {
            return new _EditStateProviderState();
        }
    }

    class _EditStateProviderState : State<EditStateProvider> {
        TextEditingController _controller;
        FocusNode _focusNode;

        public override void initState() {
            base.initState();
            this._focusNode = new FocusNode();
            this._controller = new TextEditingController("");
        }


        public override void dispose() {
            this._focusNode.dispose();
            base.dispose();
        }

        public override Widget build(BuildContext context) {
            return this.widget.builder(context, this._controller, this._focusNode);
        }
    }
}