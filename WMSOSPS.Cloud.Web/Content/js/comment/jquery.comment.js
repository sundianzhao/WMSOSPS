(function($){
	function crateCommentInfo(obj){
		/*
		 * <div class="comment-info">
			<header><img src="./images/img.jpg"></header>
			<div class="comment-right">
				<h3>匿名</h3>
				<div class="comment-content-header"><span><i class="glyphicon glyphicon-time"></i> 2017-10-17 11:42:53</span><span><i class="glyphicon glyphicon-map-marker"></i>深圳</span></div>
				<p class="content">mongodb 副本集配置副本集概念：就我的理解就是和主从复制 差不多，就是在主从复制的基础上多加了一个选举的机制。
				复制集 特点：数据一致性 主是唯一的，没有Mysql 那样的双主结构大多数原则，集群存活节点小于二分之一是集群不可写，
				只可读从库无法写入数据自动容灾通过下面的一个图来简单的了解下
				 配置过程：一、安装mongodb安装过程略，不懂得可以看前面的教程二、创建存储目录与配置文件创...</p>
				<div class="comment-content-footer">
					<div class="row">
						<div class="col-md-10">
							<span><i class="glyphicon glyphicon-pushpin"></i> 来自:win10 </span><span><i class="glyphicon glyphicon-globe"></i> chrome 55.0.2883.87</span>
						</div>
						<div class="col-md-2"><span class="reply-btn">回复</span></div>
					</div>
				</div>
				<div class="reply-list">
					<div class="reply">
						<div><a href="javascript:void(0)">匿名</a>:<a href="javascript:void(0)">@匿名</a><span>这写的是什么鬼东西。。。。</span></div>
						<p><span>2017-10-17 11:42:53</span> <span class="reply-list-btn">回复</span></p>
					</div>
				</div>
			</div>
		</div>
		 * */
		
		if(typeof(obj.time) == "undefined" || obj.time == ""){
			obj.time = getNowDateFormat();
		}
		
		var el = "<div class='comment-info'><header><img src='"+obj.img+"'></header><div class='comment-right'><h3>"+obj.replyName+"</h3>"
				+"<div class='comment-content-header'><span><i class='glyphicon glyphicon-time'></i>"+obj.time+"</span>";
		
		if(typeof(obj.address) != "undefined" && obj.browse != ""){
			el =el+"<span><i class='glyphicon glyphicon-map-marker'></i>"+obj.address+"</span>";
		}
		el = el+"</div><p class='content'>"+obj.content+"</p><div class='comment-content-footer'><div class='row'><div class='col-md-10'>";
		
		if(typeof(obj.osname) != "undefined" && obj.osname != ""){
			el =el+"<span><i class='glyphicon glyphicon-pushpin'></i> 来自:"+obj.osname+"</span>";
		}
		
		if(typeof(obj.browse) != "undefined" && obj.browse != ""){
			el = el + "<span><i class='glyphicon glyphicon-globe'></i> "+obj.browse+"</span>";
		}
		
		el = el + "</div><div class='col-md-2'><span class='btn btn-primary reply-btn'>回复</span></div></div></div><div class='reply-list'>";
		if(obj.replyBody != "" && obj.replyBody.length > 0){
			var arr = obj.replyBody;
			for(var j=0;j<arr.length;j++){
				var replyObj = arr[j];
				el = el+createReplyComment(replyObj);
			}
		}
		el = el+"</div></div></div>";
		return el;
	}
    //<span>"+reply.content+"</span>
	//返回每个回复体内容
	function createReplyComment(reply){
	    var replyEl = "<div class='reply'><div><a href='javascript:void(0)' class='replyname'>" + reply.replyName + "</a><span class='reply-time'>" + reply.time + "</span></div>"
						+ "<p class='reply-content'>" + reply.content + "</p><p> <span class='btn btn-primary reply-list-btn'>回复</span></p></div>";
		return replyEl;
	}
	function getNowDateFormat(){
		var nowDate = new Date();
		var year = nowDate.getFullYear();
		var month = filterNum(nowDate.getMonth()+1);
		var day = filterNum(nowDate.getDate());
		var hours = filterNum(nowDate.getHours());
		var min = filterNum(nowDate.getMinutes());
		var seconds = filterNum(nowDate.getSeconds());
		return year+"-"+month+"-"+day+" "+hours+":"+min+":"+seconds;
	}
	function filterNum(num){
		if(num < 10){
			return "0"+num;
		}else{
			return num;
		}
	}
	function replyClick(el, guid) {
	    var user = top.clients.user;
	    el.parent().parent().append("<div class='replybox' style='padding-top:30px'><textarea style='width: 65%; height: 120px; margin-right: 60px' placeholder='请您回复意见！' class='mytextarea' id='replyContent' ></textarea><span class='btn btn-primary send'>发送</span></div>");
	    var editor = new Simditor({
	        textarea: $('#replyContent'),
	        toolbar: ['title', 'bold', 'italic', 'underline', 'color', '|', 'ol', 'ul', 'blockquote', '|', 'indent', 'outdent']
	    });
	    //el.parent().parent().append("<div class='replybox'><textarea class='content comment-input' placeholder='请您回复意见！' style='width: 65%; height: 120px; margin-left: 38px'></textarea><span class='btn btn-primary'>发送</span></div>")
	    $(".replybox").find(".send").click(function () {
		    //var content = $(this).prev().val();
	        var content = editor.getValue();
			if(content != ""){
				var parentEl = $(this).parent().parent().parent().parent();
				var obj = new Object();
				obj.replyName = user.UserCode;
				if(el.parent().parent().hasClass("reply")){
					obj.beReplyName = el.parent().parent().find("a:first").text();
				}else{
					obj.beReplyName=parentEl.find("h3").text();
				}
				obj.content=content;
				obj.time = getNowDateFormat();
				var replyString = createReplyComment(obj);
				$(".replybox").remove();
				parentEl.find(".reply-list").append(replyString).find(".reply-list-btn:last").click(function () { alert("不能回复自己"); });
				$.ajax({
				    url: "/SystemMaintenance/Feedback/ReplySubmit",
				    data: { content: encodeURI(editor.getValue()), replyId: guid },
				    async: false,
				    dataType: "json",
				    success: function (result) {
				        if (result == true) {
				            $.modalAlert("提交成功！", "success");
				            $("#gridList").trigger("reloadGrid");
				        } else {
				            $.modalAlert("提交失败", "warning");
				        }
				    }
				});
			}else{
				alert("空内容");
			}
		});
	}
	
	
	$.fn.addCommentList=function(options){
		var defaults = {
			data:[],
			add:""
		}
		var option = $.extend(defaults, options);
		//加载数据
		if(option.data.length > 0){
			var dataList = option.data;
			var totalString = "";
			for(var i=0;i<dataList.length;i++){
				var obj = dataList[i];
				var objString = crateCommentInfo(obj);
				totalString = totalString+objString;
			}
			$(this).append(totalString).find(".reply-btn").click(function(){
				if($(this).parent().parent().find(".replybox").length > 0){
					$(".replybox").remove();
				}else{
					$(".replybox").remove();
					replyClick($(this), option.guid);
				}
			});
			$(".reply-list-btn").click(function(){
				if($(this).parent().parent().find(".replybox").length > 0){
					$(".replybox").remove();
				}else{
					$(".replybox").remove();
					replyClick($(this), option.guid);
				}
			})
		}
		
		//添加新数据
		if(option.add != ""){
			obj = option.add;
			var str = crateCommentInfo(obj);
			$(this).prepend(str).find(".reply-btn").click(function(){
			    replyClick($(this), option.guid);
			});
		}
	}
	
	
})(jQuery);