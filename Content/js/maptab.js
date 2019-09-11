$(function () {
   
    $(".tab-head span").click(function () {

        var limun = $(".tab-head span").index(this)
        $(this).addClass("on").siblings().removeClass("on");
        $(".tab-body .tab-main").eq(limun).show().siblings().hide();
        var liwidthh = $(".tab-head span").width();

        //alert("ooo:"+$(".menu-list").eq(limun).offset().left); 
        //alert(liwidthh*limun);

        $(".tab-head  p").stop(false, true).animate({ "left": liwidthh * limun + "px" }, 300)

    })

    $(".tab-min span").click(function () {
        $(this).addClass("on").siblings().removeClass("on")

    })
})