// hide contact control by default
$(document).ready(function() {
	$(".contact-button").addClass("hidden");
	$("#contact-list tr").hover(function(){
		$(this).find(".contact-button").each(function(){
			$(this).toggleClass("hidden");
		});
	});
	
});
