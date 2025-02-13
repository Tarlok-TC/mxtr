/*
	@license Angular Treeview version 0.1.6
	ⓒ 2013 AHN JAE-HA http://github.com/eu81273/angular.treeview
	License: MIT
*/
//--Old implementation
//(function (f) {
//    f.module("angularTreeview", []).directive("treeModel", function ($compile) {
//        return {
//            restrict: "A", link: function (b, h, c) {
//                //console.log(c);
//                var a = c.treeId, g = c.treeModel, e = c.nodeLabel || "AccountName", d = c.nodeChildren || "Children";

//                var e = '<ul><li data-ng-repeat="node in ' + g + '"><div class="grey-hover-box"><i class="collapsed fa fa-chevron-down" data-ng-show="node.' + d + '.length && node.collapsed" data-ng-click="' + a + '.selectNodeHead(node)"></i><i class="expanded fa fa-chevron-up" data-ng-show="node.' + d + '.length && !node.collapsed" data-ng-click="' + a + '.selectNodeHead(node)"></i><i class="normal" data-ng-hide="node.' +
//                d + '.length"></i> <span data-ng-class="node.selected" data-ng-click="' + a + '.selectNodeLabel(node)">{{node.' + e + '}}</span><span style="float:right;"><a href="{{node.EditAccountUrl}}" class="btn-sm btn-primary">Edit Account</a><span style="padding-left:10px;"><a href="{{node.AddChildAccountUrl}}" class="btn-sm btn-primary">Add Child</a></span> <span class="danger-focus-handle" style="padding-left:10px;"><button ng-click="deleteAccount(node.AccountObjectID)" class="btn-sm btn-danger">Delete</button></span></span></div><div data-ng-hide="node.collapsed" data-tree-id="' + a + '" data-tree-model="node.' + d + '" data-node-id=' + (c.nodeId || "id") + " data-node-label=" + e + " data-node-children=" + d + " ></div></li></ul>";
//                //console.log(e);               
//                a && g && (c.angularTreeview && (b[a] = b[a] || {}, b[a].selectNodeHead = b[a].selectNodeHead || function (a) { a.collapsed = !a.collapsed }, b[a].selectNodeLabel = b[a].selectNodeLabel || function (c) {
//                    b[a].currentNode && b[a].currentNode.selected &&
//                    (b[a].currentNode.selected = void 0); c.selected = "selected"; b[a].currentNode = c
//                }), h.html('').append($compile(e)(b)))
//            }
//        }
//    })
//})(angular);

//--With icons
//(function (l) {
//    l.module("angularTreeview", []).directive("treeModel", function ($compile) {
//        return {
//            restrict: "A", link: function (a, g, c) {
//                var e = c.treeModel, h = c.nodeLabel || "label", d = c.nodeChildren || "children",
//                    k = '<ul><li data-ng-repeat="node in ' + e + '"><div class="grey-hover-box"><i class="collapsed fa fa-chevron-down" data-ng-show="node.' + d + '.length && node.collapsed" data-ng-click="selectNodeHead(node, $event)"></i><i class="expanded fa fa-chevron-up" data-ng-show="node.' + d + '.length && !node.collapsed" data-ng-click="selectNodeHead(node, $event)"></i><i class="normal" data-ng-hide="node.' +
//                d + '.length"></i> <span data-ng-class="node.selected" data-ng-click="selectNodeLabel(node, $event)">{{node.' + h + '}}</span><span style="float:right;"><a href="{{node.EditAccountUrl}}" class="btn-sm btn-primary">Edit Account</a><span style="padding-left:10px;"><a href="{{node.AddChildAccountUrl}}" class="btn-sm btn-primary">Add Child</a></span><span class="danger-focus-handle" style="padding-left:10px;"><button ng-click="deleteAccount(node.AccountObjectID)" class="btn-sm btn-danger">Delete</button></span></span></div><div data-ng-hide="node.collapsed" data-tree-model="node.' + d + '" data-node-id=' + (c.nodeId || "id") + " data-node-label=" + h + " data-node-children=" + d + "></div></li></ul>"; e && e.length && (c.angularTreeview ? (a.$watch(e, function (m, b) { g.empty().html($compile(k)(a)) }, !1), a.selectNodeHead = a.selectNodeHead || function (a, b) {
//                    b.stopPropagation && b.stopPropagation(); b.preventDefault && b.preventDefault(); b.cancelBubble =
//                    !0; b.returnValue = !1; a.collapsed = !a.collapsed
//                }, a.selectNodeLabel = a.selectNodeLabel || function (c, b) { b.stopPropagation && b.stopPropagation(); b.preventDefault && b.preventDefault(); b.cancelBubble = !0; b.returnValue = !1; a.currentNode && a.currentNode.selected && (a.currentNode.selected = void 0); c.selected = "selected"; a.currentNode = c }) : g.html($compile(k)(a)))
//            }
//        }
//    })
//})(angular);


//--without icons
(function (l) {
    l.module("angularTreeview", []).directive("treeModel", function ($compile) {
        return {
            restrict: "A", link: function (a, g, c) {
                var e = c.treeModel, h = c.nodeLabel || "label", d = c.nodeChildren || "children",
                    k = '<ul><li data-ng-repeat="node in ' + e + '"><div class="grey-hover-box"><i class="collapsed" data-ng-show="node.' + d + '.length && node.collapsed" data-ng-click="selectNodeHead(node, $event)"></i><i class="expanded" data-ng-show="node.' + d + '.length && !node.collapsed" data-ng-click="selectNodeHead(node, $event)"></i><i class="normal" data-ng-hide="node.' +
                d + '.length"></i> <span data-ng-class="node.selected" data-ng-click="selectNodeLabel(node, $event)">{{node.' + h + '}}</span><span style="float:right;"><a href="{{node.EditAccountUrl}}" class="btn-sm btn-primary">Edit Account</a><span style="padding-left:10px;"><a href="{{node.AddChildAccountUrl}}" class="btn-sm btn-primary">Add Child</a></span><span class="danger-focus-handle" style="padding-left:10px;"><button ng-click="deleteAccount(node.AccountObjectID)" class="btn-sm btn-danger">Delete</button></span></span></div><div data-ng-hide="node.collapsed" data-tree-model="node.' + d + '" data-node-id=' + (c.nodeId || "id") + " data-node-label=" + h + " data-node-children=" + d + "></div></li></ul>"; e && e.length && (c.angularTreeview ? (a.$watch(e, function (m, b) { g.empty().html($compile(k)(a)) }, !1), a.selectNodeHead = a.selectNodeHead || function (a, b) {
                    b.stopPropagation && b.stopPropagation(); b.preventDefault && b.preventDefault(); b.cancelBubble =
                    !0; b.returnValue = !1; a.collapsed = !a.collapsed
                }, a.selectNodeLabel = a.selectNodeLabel || function (c, b) { b.stopPropagation && b.stopPropagation(); b.preventDefault && b.preventDefault(); b.cancelBubble = !0; b.returnValue = !1; a.currentNode && a.currentNode.selected && (a.currentNode.selected = void 0); c.selected = "selected"; a.currentNode = c }) : g.html($compile(k)(a)))
            }
        }
    })
})(angular);

//with out design 
//(function (l) {
//    l.module("angularTreeview", []).directive("treeModel", function ($compile) {
//        return {
//            restrict: "A", link: function (a, g, c) {
//                var e = c.treeModel, h = c.nodeLabel || "label", d = c.nodeChildren || "children",
//                    k = '<ul><li data-ng-repeat="node in ' + e + '"><i class="collapsed" data-ng-show="node.' + d + '.length && node.collapsed" data-ng-click="selectNodeHead(node, $event)"></i><i class="expanded" data-ng-show="node.' + d + '.length && !node.collapsed" data-ng-click="selectNodeHead(node, $event)"></i><i class="normal" data-ng-hide="node.' +
//                d + '.length"></i> <span data-ng-class="node.selected" data-ng-click="selectNodeLabel(node, $event)">{{node.' + h + '}}</span><div data-ng-hide="node.collapsed" data-tree-model="node.' + d + '" data-node-id=' + (c.nodeId || "id") + " data-node-label=" + h + " data-node-children=" + d + "></div></li></ul>"; e && e.length && (c.angularTreeview ? (a.$watch(e, function (m, b) { g.empty().html($compile(k)(a)) }, !1), a.selectNodeHead = a.selectNodeHead || function (a, b) {
//                    b.stopPropagation && b.stopPropagation(); b.preventDefault && b.preventDefault(); b.cancelBubble =
//                    !0; b.returnValue = !1; a.collapsed = !a.collapsed
//                }, a.selectNodeLabel = a.selectNodeLabel || function (c, b) { b.stopPropagation && b.stopPropagation(); b.preventDefault && b.preventDefault(); b.cancelBubble = !0; b.returnValue = !1; a.currentNode && a.currentNode.selected && (a.currentNode.selected = void 0); c.selected = "selected"; a.currentNode = c }) : g.html($compile(k)(a)))
//            }
//        }
//    })
//})(angular);

