! function(w, $) {
	w.pub = {
		popup: function() {
			$('.popup').show();
			setTimeout(function() {
				$('.popup-main').addClass('popup-main-on');
			}, 50)
		},
		popnp: function() {
			$('.popup-main').removeClass('popup-main-on');
			setTimeout(function() {
				$('.popup').hide();
			}, 200)
		}
	}
}(window, jQuery)