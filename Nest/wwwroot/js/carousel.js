document.addEventListener("DOMContentLoaded", function () {
    const track = document.querySelector('.carousel-track');
    const dots = document.querySelectorAll('.carousel-dots span');
    const slides = document.querySelectorAll('.carousel img');
    const totalSlides = slides.length;

    let currentIndex = 0;

    function moveToSlide(index) {
        track.style.transform = `translateX(-${index * 100}%)`;
        dots.forEach(dot => dot.classList.remove('active'));
        dots[index].classList.add('active');
    }

    function autoSlide() {
        currentIndex = (currentIndex + 1) % totalSlides;
        moveToSlide(currentIndex);
    }

    // Set the auto-slide interval
    setInterval(autoSlide, 3000); // Change slide every 3 seconds

    // Optional: Add click functionality to the dots
    dots.forEach(dot => {
        dot.addEventListener('click', (e) => {
            currentIndex = parseInt(e.target.dataset.index);
            moveToSlide(currentIndex);
        });
    });
});
