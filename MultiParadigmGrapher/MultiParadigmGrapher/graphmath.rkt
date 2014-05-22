(define foldl
  (lambda (proc acc lst)
    (if (null? lst)
        acc
        (foldl proc
               (proc acc (car lst))
               (cdr lst)))))

;generate list of numbers from min to max (both inclusive) in steps
(define range
  (letrec ((range-tr
            (lambda (min max step acc)
              (if (> min max) 
                  acc
                  (range-tr (+ min step) max step (cons min acc))))))
    (lambda (min max step)
      (reverse (range-tr min max step '())))))

(define floor->exact
  (lambda (x)
    (inexact->exact (floor x))))

(define step->samples
  (lambda (min max step)
    (+ 1 (floor->exact (/ (- max min) step)))))

(define samples->step
  (lambda (min max samples)
    (/ (- max min) (- samples 1))))

(define dx 0.0000000000001) ;infintesmal

(define derivative
  (lambda (f)
    (lambda (x) ;the derived function
      (/ (- (f (+ x dx)) (f x))
         dx))))

(define newton
  (let ((improve-r 
         (lambda (f r)
           (- r 
              (/ (f r) 
                 ((derivative f) r))))))
    (lambda (f r)
      (if (< (abs (f r)) dx)
          r
          (let ((improved-r (improve-r f r)))
            (newton f improved-r))))))

(define (zip p q) (map cons p q))  ;simple zip - doesn't handle lists of unequal length

(define calc-plot-data
  (lambda (f min max step)
    (let* ((x-values (range min max step))
           (y-values (map f x-values)))
      (zip x-values y-values))))

(define calc-derived-plot-data
  (lambda (f min max step)
    (calc-plot-data (derivative f) min max step)))

;;helper functions calculating the x-value used for calculating rectangle height.
(define (midpoint-x x delta-x) (+ x (/ delta-x 2)))
(define (left-x x delta-x) x )
(define (right-x x delta-x) (+ x delta-x))

;;delta-x
(define (calc-delta-x min max n)
  (/ (- max min) n))

;;Calculate heights of n rectangles at each x-value between min and max. x-proc determines the x-value used for calculating rectangle height.
;;Procedure return list of x,y pairs.
(define calc-integral-coords
  (lambda (f min max n x-proc)
    (let* ((delta-x (calc-delta-x min max n))
           (left-x-values (range min (- max delta-x) delta-x)))
      (zip left-x-values (map (lambda (x) (f (x-proc x delta-x))) left-x-values)))))

;;Procedure to be used from imperative paradigm for midpoint integrals
(define calc-midpoint-integral-coords
  (lambda (f min max n)
    (calc-integral-coords f min max n midpoint-x)))

;;Procedure to be used from imperative paradigm for left integrals
(define calc-left-integral-coords
  (lambda (f min max n)
    (calc-integral-coords f min max n left-x)))

;;Procedure to be used from imperative paradigm for right integrals
(define calc-right-integral-coords
  (lambda (f min max n)
    (calc-integral-coords f min max n right-x)))

;;Calculate definate integral (area) based on rectangle coords 
(define calc-definite-integral
  (lambda (min max n coords)
    (let* ((delta-x (calc-delta-x min max n))
          (calc-rect-area (lambda (acc xypair)
                            (+ acc (* delta-x (cdr xypair))))))
      (foldl calc-rect-area 0 coords))))

