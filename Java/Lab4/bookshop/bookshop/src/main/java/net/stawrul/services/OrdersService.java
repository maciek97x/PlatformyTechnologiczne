package net.stawrul.services;

import net.stawrul.model.Book;
import net.stawrul.model.Order;
import net.stawrul.services.exceptions.EmptyOrderException;
import net.stawrul.services.exceptions.OutOfStockException;
import net.stawrul.services.exceptions.TooManyProductsException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import java.util.*;

/**
 * Komponent (serwis) biznesowy do realizacji operacji na zamówieniach.
 */
@Service
public class OrdersService extends EntityService<Order> {

    //Instancja klasy EntityManger zostanie dostarczona przez framework Spring
    //(wstrzykiwanie zależności przez konstruktor).
    public OrdersService(EntityManager em) {

        //Order.class - klasa encyjna, na której będą wykonywane operacje
        //Order::getId - metoda klasy encyjnej do pobierania klucza głównego
        super(em, Order.class, Order::getId);
    }

    /**
     * Pobranie wszystkich zamówień z bazy danych.
     *
     * @return lista zamówień
     */
    public List<Order> findAll() {
        return em.createQuery("SELECT o FROM Order o", Order.class).getResultList();
    }

    /**
     * Złożenie zamówienia w sklepie.
     * <p>
     * Zamówienie jest akceptowane, jeśli wszystkie objęte nim produkty są dostępne (przynajmniej 1 sztuka). W wyniku
     * złożenia zamówienia liczba dostępnych sztuk produktów jest zmniejszana o jeden. Metoda działa w sposób
     * transakcyjny - zamówienie jest albo akceptowane w całości albo odrzucane w całości. W razie braku produktu
     * wyrzucany jest wyjątek OutOfStockException.
     *
     * @param order zamówienie do przetworzenia
     */
    @Transactional
    public void placeOrder(Order order) {
        if (order.getBooks() == null || order.getBooks().isEmpty()) {
            throw new EmptyOrderException();
        }
        if (order.getBooks().size() > 4) {
            throw new TooManyProductsException();
        }
        Map<UUID, Integer> changes = new HashMap<UUID, Integer>() {};
        for (Book bookStub : order.getBooks()) {
            Book book = em.find(Book.class, bookStub.getId());

            if (book.getAmount() < 1) {
                //wyjątek z hierarchii RuntineException powoduje wycofanie transakcji (rollback)
                // wycofanie zmian
                for (UUID key : changes.keySet()) {
                    Book book2 = em.find(Book.class, key);
                    int newAmount = book2.getAmount() + changes.get(key);
                    book2.setAmount(newAmount);
                }
                throw new OutOfStockException();
            } else {
                int newAmount = book.getAmount() - 1;
                book.setAmount(newAmount);
                // zapisywanie zmian do wycofania
                if (changes.containsKey(book.getId())) {
                    changes.replace(book.getId(), changes.get(book.getId()) + 1);
                }
                else {
                    changes.put(book.getId(), 1);
                }
            }
        }

        //jeśli wcześniej nie został wyrzucony wyjątek, zamówienie jest zapisywane w bazie danych
        save(order);
    }
}
