// CARROSSEL
const carousel = document.getElementById('carousel');
const prevBtn = document.getElementById('prevBtn');
const nextBtn = document.getElementById('nextBtn');
const slides = carousel.querySelectorAll('.flex-shrink-0');
const totalSlides = slides.length;
let currentIndex = 0;

function goToSlide(index) {
    currentIndex = (index + totalSlides) % totalSlides;
    const translateValue = -currentIndex * 100;
    carousel.style.transform = `translateX(${translateValue}%)`;
}

prevBtn.addEventListener('click', () => {
    goToSlide(currentIndex - 1);
});

nextBtn.addEventListener('click', () => {
    goToSlide(currentIndex + 1);
});

// SCROLL REVEAL
ScrollReveal({ reset: true });

// BANNER
ScrollReveal().reveal('.item-1', { duration: 1500, reset: false });
ScrollReveal().reveal('.item-2', { duration: 2100, reset: false });
ScrollReveal().reveal('.item-3', { duration: 2500, reset: false });
ScrollReveal().reveal('.item-4', { duration: 2500, reset: false });

// BACKGROUND NAVBAR
window.addEventListener('scroll', function () {
    var navbar = document.getElementById('navbar');
    if (window.scrollY > 0) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }
});

window.addEventListener('DOMContentLoaded', () => {
    if (window.scrollY === 0) {

        // Aplica a animação ao navbar
        ScrollReveal().reveal('.navbar', {
            origin: 'top',
            distance: '50px',
            duration: 500,
            delay: 0,
            easing: 'ease',
            opacity: 0,
            scale: 1,
            reset: false
        });

        // Define os elementos e configurações para animação
        const elements = [
            { selector: '.nav-item-1', duration: 1000 },
            { selector: '.nav-item-2', duration: 1000 },
            { selector: '.nav-item-3', duration: 1500 },
            { selector: '.nav-item-4', duration: 2000 },
            { selector: '.nav-item-5', duration: 2500 }
        ];

        // Aplica a animação aos elementos do navbar
        elements.forEach((el, index) => {
            ScrollReveal().reveal(el.selector, {
                origin: 'top',
                distance: '50px',
                duration: el.duration,
                delay: index * 100, // Atraso crescente para cada elemento
                easing: 'ease',
                opacity: 0,
                scale: 1,
                reset: false
            });
        });

        // SECTION 1
        const section1Elements = [
            { selector: '.section-1-item-1', origin: 'left', delay: 0 },
            { selector: '.section-1-item-2', origin: 'right', delay: 200 }
        ];

        // Aplica a animação aos elementos da seção 1
        section1Elements.forEach(el => {
            ScrollReveal().reveal(el.selector, {
                origin: el.origin,
                distance: '500px',
                duration: 1500,
                delay: el.delay,
                easing: 'ease',
                opacity: 0,
                scale: 1,
                reset: false
            });
        });
    }
});

// MAIN
ScrollReveal().reveal('.main-item-1', {
    origin: 'right',
    distance: '600px',
    duration: 1500,
    delay: 200,
    easing: 'ease',
    opacity: 0,
    scale: 1,
    reset: false
});

ScrollReveal().reveal('.main-item-2', {
    origin: 'left',
    distance: '600px',
    duration: 1500,
    delay: 200,
    easing: 'ease',
    opacity: 0,
    scale: 1,
    reset: false
});
